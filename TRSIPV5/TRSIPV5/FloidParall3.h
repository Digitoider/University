#include "mpi.h"

#include "stdio.h"

#include "stdlib.h"

#include "limits.h"

#include "omp.h"

#include <random>

#include <algorithm>

#include <fstream>

#include <iostream>

#include "chrono"

#define OP_CLOSE 0

#define OP_FLOYD 1

#define VERTEX_COUNT 100

#define ITERATIONS_COUNT 100

#define INF 9999999

#define ROOT 0

using namespace std;

using namespace chrono;

MPI_Request request;

MPI_Status status;

int mpi_rank, mpi_size, mpi_op;

unsigned int vertex_count = VERTEX_COUNT;

void master();

void slave();

long long test(void(*timed_function)(void **), long long *full_time);

void floydSequential(void **arg);

void floydParallelMaster(void **arg);

void floydParallelSlave();

void op(int op_code);

long long

timer(void(*before)(void **), void(*timed_function)(void **), void(*after)(void **), void **arg, int iterations,

	long long *time_full);

void printMatrix(int *matrix);

void *vmem(const size_t size);

int *imem(const int size);

void free_mem(void *ptr);

int main(int argc, char **argv) {

	srand((unsigned int)time(NULL));

	MPI_Init(&argc, &argv);

	MPI_Comm_size(MPI_COMM_WORLD, &mpi_size);

	MPI_Comm_rank(MPI_COMM_WORLD, &mpi_rank);

	mpi_rank == ROOT ? master() : slave();

	MPI_Finalize();

	return 0;

}

void master() {

	long long sequential_ns, parallel_ns;

	cout << "Parallel:" << endl;
	for (vertex_count = 5; vertex_count <= 13; vertex_count++) {
		parallel_ns = test(floydParallelMaster, NULL);
		cout << "amount of nodes: " << vertex_count << "\t" << "\t Time in seconds: " << parallel_ns/1000000000.0 << endl;
	}
	cout << "Sequential:" << endl;
	for (vertex_count = 5; vertex_count <= 13; vertex_count++) {
		sequential_ns = test(floydSequential, NULL);
		cout << "amount of nodes: " << vertex_count << "\t" << "\t Time in seconds: " << sequential_ns / 1000000000.0 << endl;
	}

	op(OP_CLOSE);

}

void slave() {

	while (true) {

		MPI_Bcast(&mpi_op, 1, MPI_INT, ROOT, MPI_COMM_WORLD);

		switch (mpi_op) {

		case OP_FLOYD:

			floydParallelSlave();

			break;

		case OP_CLOSE:

		default:

			return;

		}

	}

}

void op(int op_code) {

	MPI_Bcast(&op_code, 1, MPI_INT, ROOT, MPI_COMM_WORLD);

}

int v(int i, int j) {

	return i * vertex_count + j;

}

void generate_matrix(void **arg) {

	int *array = *((int **)arg);

	for (int i = 0; i < vertex_count; i++) {

		for (int j = 0; j < vertex_count; j++) {

			array[v(i, j)] = i == j ? 0 : rand() % 2 ? rand() % INF : INF;

		}

	}

}

void post(void **arg) {}

long long test(void(*timed_function)(void **), long long *full_time) {

	int *array = imem(vertex_count * vertex_count);

	long long time = timer(generate_matrix, timed_function, post, (void **) &array, ITERATIONS_COUNT, full_time);

	free_mem(array);

	return time;

}

void floydSequential(void **arg) {

	int *array = *((int **)arg);

	for (int k = 0; k < vertex_count; k++)

		for (int i = 0; i < vertex_count; i++)

			for (int j = 0; j < vertex_count; j++)

				array[v(i, j)] = min(array[v(i, j)], array[v(i, k)] + array[v(k, j)]);

}

void floydParallelMaster(void **arg) {

	int *matrix = *((int **)arg);

	op(OP_FLOYD);

	MPI_Bcast(&vertex_count, 1, MPI_INT, ROOT, MPI_COMM_WORLD);

	int workSize = mpi_size - 1;

	int *rowCounts = (int *)malloc(sizeof(int) * workSize);

	for (int i = 0; i < workSize - 1; i++) {

		rowCounts[i] = vertex_count / workSize;

	}

	if (vertex_count % workSize == 0) {

		rowCounts[workSize - 1] = vertex_count / workSize;

	}

	else {

		rowCounts[workSize - 1] = vertex_count - ((vertex_count / workSize) * (workSize - 1));

	}

	for (int i = 1; i < mpi_size; i++) {

		MPI_Send(&rowCounts[i - 1], 1, MPI_INT, i, 0, MPI_COMM_WORLD);

	}

	for (int k = 0; k < vertex_count; k++) {

		for (int p = 1; p < mpi_size; p++) {

			MPI_Isend(&matrix[k * vertex_count], vertex_count, MPI_INT, p, 0, MPI_COMM_WORLD, &request);

		}

		int num = 0;

		for (int i = 0; i < workSize; i++) {

			for (int j = 0; j < rowCounts[i]; j++) {

				MPI_Isend(&matrix[num * vertex_count], vertex_count, MPI_INT, i + 1, 0, MPI_COMM_WORLD, &request);

				num++;

			}

		}

		num = 0;

		for (int i = 0; i < workSize; i++) {

			for (int j = 0; j < rowCounts[i]; j++) {

				MPI_Recv(&matrix[num * vertex_count], vertex_count, MPI_INT, i + 1, 0, MPI_COMM_WORLD, &status);

				num++;

			}

		}

	}

}

void floydParallelSlave() {

	MPI_Bcast(&vertex_count, 1, MPI_INT, ROOT, MPI_COMM_WORLD);

	int rowCount;

	MPI_Recv(&rowCount, 1, MPI_INT, ROOT, 0, MPI_COMM_WORLD, &status);

	int *kRow = imem(vertex_count);

	int *rows = imem(rowCount * vertex_count);

	for (int k = 0; k < vertex_count; k++) {

		MPI_Recv(kRow, vertex_count, MPI_INT, ROOT, 0, MPI_COMM_WORLD, &status);

		for (int i = 0; i < rowCount; i++) {

			MPI_Recv(&rows[i * vertex_count], vertex_count, MPI_INT, ROOT, 0, MPI_COMM_WORLD, &status);

		}

		for (int i = 0; i < rowCount; i++) {

			for (int j = 0; j < vertex_count; j++) {

				rows[v(i, j)] = min(

					rows[v(i, j)],

					rows[v(i, k)] + kRow[j]

				);

			}

		}

		for (int i = 0; i < rowCount; i++) {

			MPI_Send(&rows[i * vertex_count], vertex_count, MPI_INT, ROOT, 0, MPI_COMM_WORLD);

		}

	}

	free_mem(kRow);

	free_mem(rows);

}

long long

timer(void(*before)(void **), void(*timed_function)(void **), void(*after)(void **), void **arg, int iterations,

	long long *time_full) {

	long long time_only = 0;

	if (time_full != NULL) {

		(*time_full) = 0;

	}

	for (int i = 0; i < iterations; i++) {

		auto before_time = high_resolution_clock::now();

		before(arg);

		auto start_time = high_resolution_clock::now();

		timed_function(arg);

		auto end_time = high_resolution_clock::now();

		after(arg);

		auto after_time = high_resolution_clock::now();

		time_only += duration_cast<nanoseconds>(end_time - start_time).count();

		if (time_full != NULL) {

			(*time_full) += duration_cast<nanoseconds>(after_time - before_time).count();

		}

	}

	return time_only;

}

void printMatrix(int *matrix) {

	for (int i = 0; i < vertex_count; i++) {

		for (int j = 0; j < vertex_count; j++) {

			cout << matrix[i * vertex_count + j] << " ";

		}

		cout << endl;

	}

}

void *vmem(const size_t size) {

	return malloc(size);

}

int *imem(const int size) {

	return (int *)vmem(sizeof(int) * size);

}

void free_mem(void *ptr) {

	if (ptr != nullptr) {

		free(ptr);

	}

}