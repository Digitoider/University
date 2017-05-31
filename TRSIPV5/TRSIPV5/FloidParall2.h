#include <iostream>
#include <algorithm>
#include <fstream>
#include <time.h>

#include "mpi.h"

//������������ �������� ���� = 100
#define INF 101
#define ROOT_PROC 0
#define ROW_COUNTS_TAG 200
#define RECV_LINE_TAG 201
#define RECV_ROWS_TAG 202
#define RECV_ALL_BACK_TAG 203
using namespace std;

void printMatrix(int *matrix, int amountOfNodes) {
	for (int i = 0; i < amountOfNodes; i++) {
		for (int j = 0; j < amountOfNodes; j++) {
			if (matrix[i*amountOfNodes + j] == INF) cout << "0" << " ";
			else cout << matrix[i*amountOfNodes + j] << " ";
		}
		cout << endl;
	}
}
void GenerateMatrix(int * matrix, int amountOfNodes) {
	for (int i = 0; i < amountOfNodes; i++) {
		for (int j = 0; j < amountOfNodes; j++) {
			if (i == j) {
				matrix[i*amountOfNodes + j] = 0;
			}
			else {
				if (rand() % 20 < 10) {
					matrix[i*amountOfNodes + j] = rand() % INF;
				}
				else {
					matrix[i*amountOfNodes + j] = INF;
				}
			}
		}
	}
}
void floydSequential(int*array, int vertex_count) {

	for (int k = 0; k < vertex_count; k++)

		for (int i = 0; i < vertex_count; i++)

			for (int j = 0; j < vertex_count; j++)

				array[i * vertex_count + j] = min(array[i * vertex_count + j], array[i * vertex_count + j] + array[i * vertex_count + j]);

}
void Floid(int vertex_count) {
	int rank, size;
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);

	int amountOfNodes;
	MPI_Request request;
	MPI_Status status;

	if (rank == ROOT_PROC) {
		amountOfNodes = vertex_count;
		int *matrix;

		//������� ��������� � ������ ����� �����(INF - ����� ���, 0 ����� � ����)
		matrix = (int *)malloc(sizeof(int) * (amountOfNodes * amountOfNodes));

		for (int i = 0; i < amountOfNodes; i++) {
			for (int j = 0; j < amountOfNodes; j++) {
				if (i == j) {
					matrix[i*amountOfNodes + j] = 0;
				}
				else {
					if (rand() % 20 < 10) {
						matrix[i*amountOfNodes + j] = rand() % INF;
					}
					else {
						matrix[i*amountOfNodes + j] = INF;
					}
				}
			}
		}
		cout << endl << "N: " << amountOfNodes << endl;
		double st = MPI_Wtime();
		floydSequential(matrix, amountOfNodes);
		cout << "Sequential(time in seconds): " << MPI_Wtime() - st << endl;

		double startTime = MPI_Wtime();
		//cout << "Root process. Number of vertex sent to other processes" << endl;
		//������� ��������� ���������� ������
		MPI_Bcast(&amountOfNodes, 1, MPI_INT, ROOT_PROC, MPI_COMM_WORLD);

		//cout << "Root process. Print first matrix:" << endl;
		//printMatrix(matrix, amountOfNodes);

		//������� ������� �������� ����� ���������� ����� �������

		//��������� �� ������� ����� ������ ������� ��������
		int workSize = size - 1;//(���� �� ������)
		int *rowCounts = (int*)malloc(sizeof(int) * workSize);

		for (int i = 0; i < workSize - 1; i++) {
			rowCounts[i] = amountOfNodes / workSize;
		}
		//���� ������� ������� �� ���������� ������� ��
		if (amountOfNodes % workSize == 0) {
			rowCounts[workSize - 1] = amountOfNodes / workSize;
		}
		//���� �� �������, �� � ��������� ��������� �������
		else {
			rowCounts[workSize - 1] = amountOfNodes - ((amountOfNodes / workSize) * (workSize - 1));
		}

		//cout << "Root process. Row counts sent to other processes" << endl;
		//������� ������� �������� ������� ��� ����� �����
		for (int i = 1; i < size; i++) {
			MPI_Send(&rowCounts[i - 1], 1, MPI_INT, i, ROW_COUNTS_TAG, MPI_COMM_WORLD);
		}

		//cout << "Root process. Main loop start" << endl;
		//�������� ���� ��������� ������
		for (int k = 0; k < amountOfNodes; k++) {
			//������� k-�� ������ ���� ���������(����� ����)
			for (int p = 1; p < size; p++) {
				MPI_Isend(&matrix[k*amountOfNodes], amountOfNodes, MPI_INT, p, RECV_LINE_TAG, MPI_COMM_WORLD, &request);
			}

			//������ ������� �������� ��� ������ �������
			int num = 0; //����� ������
			for (int i = 0; i < workSize; i++) { //����� �� ������� � ���-��� �����
				for (int j = 0; j < rowCounts[i]; j++) { //������ ������ ���������� �����
					MPI_Isend(&matrix[num*amountOfNodes], amountOfNodes, MPI_INT, i + 1, RECV_ROWS_TAG, MPI_COMM_WORLD, &request);
					num++;
				}
			}

			//�������� �� ��������� ������, ���������� �� ���� ��������
			num = 0; //����� ������
			for (int i = 0; i < workSize; i++) { //����� �� ������� � ���-��� �����
				for (int j = 0; j < rowCounts[i]; j++) { //��������� ������ ���������� �����
					MPI_Recv(&matrix[num*amountOfNodes], amountOfNodes, MPI_INT, i + 1, RECV_ALL_BACK_TAG, MPI_COMM_WORLD, &status);
					//cout << "recieved in root" << endl;
					num++;
				}
			}
		}
		cout << "Parallel (time in seconds): " << MPI_Wtime() - startTime << endl;
		//cout << "Root process. Print final matrix:" << endl;
		//printMatrix(matrix, amountOfNodes);

		free(matrix);
	}
	else {
		//�������� ���������� ������
		MPI_Bcast(&amountOfNodes, 1, MPI_INT, ROOT_PROC, MPI_COMM_WORLD);

		//�������� ���������� �����, ������� ����� ����� ��������
		int rowCount;
		MPI_Recv(&rowCount, 1, MPI_INT, ROOT_PROC, ROW_COUNTS_TAG, MPI_COMM_WORLD, &status);
		//cout << rank << " : " << rowCount << endl;

		//�������� ������
		int *kRow = (int *)malloc(sizeof(int) * amountOfNodes);
		int *rows = (int *)malloc(sizeof(int) * (rowCount * amountOfNodes));

		//�������� ���� ��������� ������
		for (int k = 0; k < amountOfNodes; k++) {
			//�������� k-� ������
			MPI_Recv(kRow, amountOfNodes, MPI_INT, ROOT_PROC, RECV_LINE_TAG, MPI_COMM_WORLD, &status);

			//�������� ���� ������ �� �������
			for (int i = 0; i < rowCount; i++) {
				MPI_Recv(&rows[i*amountOfNodes], amountOfNodes, MPI_INT, ROOT_PROC, RECV_ROWS_TAG, MPI_COMM_WORLD, &status);
			}

			//�������� ������ ������ � �������(������������ ������)
			for (int i = 0; i < rowCount; i++) {
				for (int j = 0; j < amountOfNodes; j++) {
					rows[i*amountOfNodes + j] = min(
						rows[i*amountOfNodes + j],
						rows[i*amountOfNodes + k] + kRow[j]
					);
				}
			}

			//��������� ��������� ����
			for (int i = 0; i < rowCount; i++) {
				MPI_Send(&rows[i*amountOfNodes], amountOfNodes, MPI_INT, ROOT_PROC, RECV_ALL_BACK_TAG, MPI_COMM_WORLD);
			}
		}

		free(kRow);
		free(rows);
	}
}
int main(int argc, char** argv) {
	srand(time(NULL));
	MPI_Init(&argc, &argv);

	//Floid(10);
	//Floid(50);
	//Floid(100);
	Floid(500);

	MPI_Finalize();
}
//mpiexec - n 4 "C:\Users\Alexander\Documents\Visual Studio 2015\Projects\TRSIPV5\Debug\TRSIPV5.exe"