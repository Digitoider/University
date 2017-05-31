#include "stdafx.h"
#include <iostream>
#include <mpi.h>
#include <fstream>
#include <string.h>
#include <algorithm>
#include "Qsort.h"


#define SENDTAG 0
#define RECIEVE_REST_ARRAY_TAG 800
#define ARRAY_SEND_TAG 801
#define GATHER_LEFT_COUNTS 802
#define GATHER_ALL_LEFT_PARTS 803
#define SORT_IS_DONE 804
#define FORM_LEFT_RIGHT_ARRAYS_TAG 1
#define ROOT 0

int* RandomizeMas(int n) {
	int *mas = (int*)malloc(sizeof(int) * n);
	for (int i = 0; i < n; i++) {
		mas[i] = rand();
	}
	return mas;
}
int Max(int *mas, int left, int right) {
	int maxE = mas[left];
	for (int i = left + 1; i < right; i++) {
		if (maxE < mas[i]) {
			maxE = mas[i];
		}
	}
	return maxE;
}
int Min(int *mas, int left, int right) {
	int minE = mas[left];
	for (int i = left + 1; i < right; i++) {
		if (minE > mas[i]) {
			minE = mas[i];
		}
	}
	return minE;
}
int DefineBearingElem(int *arr, int length) {
	return (Min(arr, 0, length) + Max(arr, 0, length)) / 2;
}
int myRank, amountOfProcesses, currentIteration;
int* mas; int masLength;
void printMas(int* arr, int n, bool addEndLine = true) {
	for (int i = 0; i < n; i++) {
		std::cout << arr[i] << " ";
	}
	if (addEndLine) {
		std::cout << std::endl;
	}
}
void CountPrefixSum(int *arr, int length) {
	for (int i = 2; i < length; i++) {
		arr[i] += arr[i - 1];
	}
}
MPI_Comm CreateComm(int i) {
	MPI_Comm comm;
	int color = 0;
	if (myRank >= i) {
		color = 1;
	}
	int * ranks = new int[i];

	for (int k = 0; k < i; k++) {
		ranks[k] = k;
	}
	MPI_Group worldGroup, restGroup;
	MPI_Comm_group(MPI_COMM_WORLD, &worldGroup);
	MPI_Group_excl(worldGroup, i, ranks, &restGroup);
	MPI_Comm_create(MPI_COMM_WORLD, restGroup, &comm);

	delete ranks;
	return comm;
}

void Scatter(MPI_Comm graphComm) {
	int commSize, rank;
	MPI_Comm_rank(graphComm, &rank);
	MPI_Comm_size(graphComm, &commSize);
	int length = masLength / commSize;//количество элементов для каждого процесса

	int offset;

	for (int i = 1; i < commSize - 1; i++) {
		offset = i*length;
		MPI_Send((mas + offset), length, MPI_INT, i, ARRAY_SEND_TAG, graphComm);
	}
	offset = (commSize - 1)*length;
	MPI_Send((mas + offset), masLength - offset, MPI_INT, commSize - 1, ARRAY_SEND_TAG, graphComm);
}
void ReorderArray(int *arr, int &l, int r, int piv)
{
	while (l <= r)
	{
		while (arr[l] < piv)
			l++;
		while (arr[r] > piv)
			r--;
		if (l <= r)
			std::swap(arr[l++], arr[r--]);
	}
}

void Copy(int *dest, int* source, int length) {
	for (int i = 0; i < length; i++) {
		dest[i] = source[i];
	}
}
void defineLeftCounts(int *leftCounts, int left, int commSize, MPI_Comm graphComm) {
	leftCounts[0] = left;
	for (int i = 1; i < commSize; i++) {
		MPI_Recv((leftCounts + i), 1, MPI_INT, i, GATHER_LEFT_COUNTS, graphComm, MPI_STATUS_IGNORE);
	}
}
void defineRightCounts(int *rightCounts, int* leftCounts, int left, int commSize, int length, MPI_Comm graphComm) {
	for (int i = 0; i < commSize - 1; i++) {
		rightCounts[i] = length - leftCounts[i];
	}
	rightCounts[commSize - 1] = length + masLength % commSize - leftCounts[commSize - 1];
}
void defineDispls(int *leftCounts, int*rightCounts, int*leftDispls, int*rightDispls, int commSize) {
	Copy((leftDispls + 1), leftCounts, commSize);
	Copy((rightDispls + 1), rightCounts, commSize);

	CountPrefixSum(leftDispls, commSize + 1);
	CountPrefixSum(rightDispls, commSize + 1);

	leftDispls[0] = rightDispls[0] = 0;

	for (int i = 0; i < commSize + 1; i++) {
		rightDispls[i] += leftDispls[commSize];
	}
}
void MQSort(MPI_Comm graphComm) {
	int commRank, commSize;
	MPI_Comm_rank(graphComm, &commRank);
	MPI_Comm_size(graphComm, &commSize);


	int bearingElem;
	if (commRank == ROOT) {
		bearingElem = DefineBearingElem(mas, masLength);
		MPI_Bcast(&bearingElem, 1, MPI_INT, ROOT, graphComm);

		Scatter(graphComm);

		int left = 0, length = masLength / commSize;
		ReorderArray(mas, left, length - 1, bearingElem);

		int *leftCounts = (int*)malloc(sizeof(int) * commSize);
		int *rightCounts = (int*)malloc(sizeof(int) * commSize);
		defineLeftCounts(leftCounts, left, commSize, graphComm);
		defineRightCounts(rightCounts, leftCounts, left, commSize, length, graphComm);
		

		int *leftDispls = (int*)malloc(sizeof(int) * (commSize + 1));
		int *rightDispls = (int*)malloc(sizeof(int) * (commSize + 1));
		
		defineDispls(leftCounts, rightCounts, leftDispls, rightDispls, commSize);

		int* rightPart = (int*)malloc(sizeof(int) * (length - left));
		for (int i = 0; i < length - left; i++) {
			rightPart[i] = mas[i + left];
		}

		MPI_Gatherv(mas, length, MPI_INT, mas, leftCounts, leftDispls, MPI_INT, ROOT, graphComm);

		MPI_Gatherv(rightPart, length - left, MPI_INT, mas, rightCounts, rightDispls, MPI_INT, ROOT, graphComm);

		// отправить правую часть в ROOT+1 MPI_COMM_WORLD
		int offset = leftDispls[commSize];
		MPI_Send((mas + offset), masLength - offset, MPI_INT, myRank + 1, RECIEVE_REST_ARRAY_TAG, MPI_COMM_WORLD);

		Qsort(mas, 0, leftDispls[commSize] - 1);

		masLength = leftDispls[commSize];
		if (myRank != ROOT) {
			MPI_Send(mas, leftDispls[commSize], MPI_INT, 0, GATHER_ALL_LEFT_PARTS, MPI_COMM_WORLD);
		}
		else {
			masLength = leftCounts[commSize - 1] + leftDispls[commSize - 1];
		}
	}
	else {
		MPI_Bcast(&bearingElem, 1, MPI_INT, ROOT, graphComm);
		MPI_Status st;
		MPI_Probe(ROOT, ARRAY_SEND_TAG, graphComm, &st);
		int arrLength;
		MPI_Get_count(&st, MPI_INT, &arrLength);

		int*arr = (int*)malloc(sizeof(int) * arrLength);
		MPI_Recv(arr, arrLength, MPI_INT, ROOT, ARRAY_SEND_TAG, graphComm, MPI_STATUS_IGNORE);

		int left = 0;
		ReorderArray(arr, left, arrLength - 1, bearingElem);
		if (left > arrLength) {
			left = arrLength;
		}

		//передадим индекс левой части
		MPI_Send(&left, 1, MPI_INT, ROOT, GATHER_LEFT_COUNTS, graphComm);

		int *buf = new int;
		//отправим левую часть
		MPI_Gatherv(arr, left, MPI_INT, buf, buf, buf, MPI_INT, ROOT, graphComm);

		//отправим правую часть
		MPI_Gatherv((arr + left), arrLength - left, MPI_INT, buf, buf, buf, MPI_INT, ROOT, graphComm);
	}
}

int main(int argc, char ** argv) {
	MPI_Init(&argc, &argv);
	MPI_Comm_rank(MPI_COMM_WORLD, &myRank);
	MPI_Comm_size(MPI_COMM_WORLD, &amountOfProcesses);

	if (argc == 1) {
		if (myRank == ROOT) {
			std::cout << "Not enough arguments; Amount of elements must be specified";
		}
		MPI_Finalize();
		return -1;
	}
	double startTime, endTime;
	if (myRank == ROOT) {
		std::cout << "amount of args: " << argc << std::endl;
		for (int i = 0; i < argc; i++) {
			std::cout << argv[i] << std::endl;
		}
		masLength = atoi(argv[1]);
		RandomizeMas(masLength);
		if (argc > 3 && strcmp(argv[2], "-p") == 0) {
			std::cout << "generated array: "; printMas(mas, masLength);
		}
	}
	MPI_Barrier(MPI_COMM_WORLD);
	startTime = MPI_Wtime();

	for (int i = 0; i < amountOfProcesses - 1; i++) {
		currentIteration = i;
		MPI_Comm graphComm = CreateComm(i);
		MPI_Barrier(MPI_COMM_WORLD);
		MQSort(graphComm);
		if (i == 0 && myRank == ROOT && argc > 2 && strcmp(argv[2], "-p") == 0) {
			printMas(mas, masLength, false);
		}
		if (i > 0 && myRank == ROOT) {
			MPI_Status status;
			MPI_Probe(i, GATHER_ALL_LEFT_PARTS, MPI_COMM_WORLD, &status);
			int length = 0;
			MPI_Get_count(&status, MPI_INT, &length);

			int* arr = (int*)malloc(sizeof(int) * length);
			MPI_Recv(arr, length, MPI_INT, i, GATHER_ALL_LEFT_PARTS, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
			//std::cout << "      Gathered sorted part with masLength: " << length << " from rank " << i << std::endl;
			if (argc > 2 && strcmp(argv[2], "-p") == 0) {
				printMas(arr, length, false);
			}
		}
		char *msg = (char*)malloc(sizeof(char) * 10);
		if (myRank == i + 1) {
			MPI_Status status;
			MPI_Probe(myRank - 1, RECIEVE_REST_ARRAY_TAG, MPI_COMM_WORLD, &status);
			MPI_Get_count(&status, MPI_INT, &masLength);
			if (masLength == 0)  break;

			mas = (int*)malloc(sizeof(int) * masLength);
			MPI_Recv(mas, masLength, MPI_INT, myRank - 1, RECIEVE_REST_ARRAY_TAG, MPI_COMM_WORLD, MPI_STATUS_IGNORE);
			if (masLength < (amountOfProcesses - i) * 2 || i == amountOfProcesses - 2) {
				Qsort(mas, 0, masLength - 1);
				if (argc > 2 && strcmp(argv[2], "-p") == 0) {
					printMas(mas, masLength, false);
				}
				strncpy_s(msg, sizeof(char) * 10, "done", 10);
			}
			else {
				strncpy_s(msg, sizeof(char) * 10, "isnotdone", 10);
			}
		}
		MPI_Barrier(MPI_COMM_WORLD);
		
		MPI_Bcast(msg, 10, MPI_CHAR, i + 1, MPI_COMM_WORLD);
		if (strcmp(msg, "done") == 0) {
			break;
		}
	}

	endTime = MPI_Wtime();
	MPI_Finalize();
	std::cout << "sort time in seconds: " << endTime - startTime;
	return endTime - startTime;
}
