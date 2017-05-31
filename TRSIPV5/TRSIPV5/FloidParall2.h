#include <iostream>
#include <algorithm>
#include <fstream>
#include <time.h>

#include "mpi.h"

//Максимальное значение веса = 100
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

		//Матрица смежности с весами ребер графа(INF - ребра нет, 0 ребро в себя)
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
		//Раздать процессам количество вершин
		MPI_Bcast(&amountOfNodes, 1, MPI_INT, ROOT_PROC, MPI_COMM_WORLD);

		//cout << "Root process. Print first matrix:" << endl;
		//printMatrix(matrix, amountOfNodes);

		//Раздать каждому процессу нужно количество строк матрицы

		//Вычислить по сколько строк отдать каждому процессу
		int workSize = size - 1;//(руту не отдаем)
		int *rowCounts = (int*)malloc(sizeof(int) * workSize);

		for (int i = 0; i < workSize - 1; i++) {
			rowCounts[i] = amountOfNodes / workSize;
		}
		//Если поровну делится то последнему столько же
		if (amountOfNodes % workSize == 0) {
			rowCounts[workSize - 1] = amountOfNodes / workSize;
		}
		//Если не поровну, то в последний скидываем остатки
		else {
			rowCounts[workSize - 1] = amountOfNodes - ((amountOfNodes / workSize) * (workSize - 1));
		}

		//cout << "Root process. Row counts sent to other processes" << endl;
		//Сказать каждому процессу сколько ему ждать строк
		for (int i = 1; i < size; i++) {
			MPI_Send(&rowCounts[i - 1], 1, MPI_INT, i, ROW_COUNTS_TAG, MPI_COMM_WORLD);
		}

		//cout << "Root process. Main loop start" << endl;
		//Основной цикл алгоритма Флойда
		for (int k = 0; k < amountOfNodes; k++) {
			//Раздаем k-ую строку всем процессам(кроме рута)
			for (int p = 1; p < size; p++) {
				MPI_Isend(&matrix[k*amountOfNodes], amountOfNodes, MPI_INT, p, RECV_LINE_TAG, MPI_COMM_WORLD, &request);
			}

			//Отдать каждому процессу его строки матрицы
			int num = 0; //Номер строки
			for (int i = 0; i < workSize; i++) { //Бежим по массиву с кол-вом строк
				for (int j = 0; j < rowCounts[i]; j++) { //Отдаем нужное количество строк
					MPI_Isend(&matrix[num*amountOfNodes], amountOfNodes, MPI_INT, i + 1, RECV_ROWS_TAG, MPI_COMM_WORLD, &request);
					num++;
				}
			}

			//Получаем от процессов строки, измененные на этой итерации
			num = 0; //Номер строки
			for (int i = 0; i < workSize; i++) { //Бежим по массиву с кол-вом строк
				for (int j = 0; j < rowCounts[i]; j++) { //Принимаем нужное количество строк
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
		//Получить количество вершин
		MPI_Bcast(&amountOfNodes, 1, MPI_INT, ROOT_PROC, MPI_COMM_WORLD);

		//Получить количество строк, которые нужно будет изменить
		int rowCount;
		MPI_Recv(&rowCount, 1, MPI_INT, ROOT_PROC, ROW_COUNTS_TAG, MPI_COMM_WORLD, &status);
		//cout << rank << " : " << rowCount << endl;

		//Выделить память
		int *kRow = (int *)malloc(sizeof(int) * amountOfNodes);
		int *rows = (int *)malloc(sizeof(int) * (rowCount * amountOfNodes));

		//Основной цикл алгоритма Флойда
		for (int k = 0; k < amountOfNodes; k++) {
			//Получить k-ю строку
			MPI_Recv(kRow, amountOfNodes, MPI_INT, ROOT_PROC, RECV_LINE_TAG, MPI_COMM_WORLD, &status);

			//Получить свои строки из матрицы
			for (int i = 0; i < rowCount; i++) {
				MPI_Recv(&rows[i*amountOfNodes], amountOfNodes, MPI_INT, ROOT_PROC, RECV_ROWS_TAG, MPI_COMM_WORLD, &status);
			}

			//Изменить нужные строки в матрице(параллельная работа)
			for (int i = 0; i < rowCount; i++) {
				for (int j = 0; j < amountOfNodes; j++) {
					rows[i*amountOfNodes + j] = min(
						rows[i*amountOfNodes + j],
						rows[i*amountOfNodes + k] + kRow[j]
					);
				}
			}

			//Отправить изменения руту
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