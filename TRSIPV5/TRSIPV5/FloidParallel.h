
#include <stdio.h>

#include <string.h>

#include <stdlib.h>

#include <errno.h>

#include <time.h>

#include <stdarg.h>

#include <math.h>

#include <mpi.h>

// --- ��������� ��������� ---

// ��������� ������ �������

#define DEBUG

// ����� �������� �����

#define TEST_COUNT 5

// ����������� �� ������������ ���� ����

#define MIN_WEIGHT 1

#define MAX_WEIGHT 9

// ��������� �����

#define DENSITY 0.80

// ����� ������ �����

#define V_SIZE 100

// ��� ����� ������ � �����

typedef int vsize;

// ��� ����� ���� �����

typedef int weight;

// ���� � ����� �����

#define LOG_PATH "log.txt"

// ����������� ������������ ���� �����

#define INF 100

// --- ��������� MPI ---

// ������ �������� (������������) ��������

#define MAIN_PROC 0

// ��� ����� ������ � �����

#define MPI_VSIZE MPI_INT

// ��� ����� ���� �����

#define MPI_WEIGHT MPI_INT

weight get_random(weight a, weight b, float density, weight inf);

vsize gen_random(vsize a, vsize b);

void print_log(const char *message, ...);

void print_err(const char *message);

int main(int argc, char *argv[])

{

	// ����� ��������� � ����� �������� ��������

	int procNum = 1;

	int procRank = MAIN_PROC;

	// ����� ������ ������ �������� ��������

	double startTime;

	vsize i, j, k, p, h;

	// ����� ������ ����� G=(V,E), |V|=n

	vsize n = V_SIZE;

	// ������ ��������� � �������� ������ �������� ����

	vsize Vstart, Vend;

	MPI_Init(&argc, &argv);

	MPI_Comm_size(MPI_COMM_WORLD, &procNum);

	MPI_Comm_rank(MPI_COMM_WORLD, &procRank);

	MPI_Status status;

	// ���� � ������ ������� ���������, �������� � ����������

	const char *M_path = "M.txt";

	const char *Q_path = "query.txt";

	const char *R_path = "result.txt";

	FILE *Mstream;

	FILE *Qstream;

	FILE *Rstream;

	// ���������� ������� ������� ������ �������� ��������

	double *results;

	vsize *Wrows = NULL;

	vsize *Nrows = NULL;

	if (procRank == MAIN_PROC)

	{

		// ��������� ������ ��� ���������� ������� �������

		results = (double *)malloc(TEST_COUNT * sizeof(double));

		if (results == NULL)

		{

			print_err("�� ������� �������� ������");

			MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

			return 0;

		}

	}

	// ��������� ���� TEST_COUNT ���

	for (h = 0; h < TEST_COUNT; h++)

	{

		MPI_Barrier(MPI_COMM_WORLD);

		if (procRank == MAIN_PROC)

		{

			// ��������� ����� ������ ������ �������� ��������

			startTime = MPI_Wtime();

			// �������� ������� ����� � �������� ��������� � ��������

			Mstream = fopen(M_path, "r");

			Qstream = fopen(Q_path, "r");

			// ��������� ������ ��� �������� ������

			if (Mstream == NULL || Qstream == NULL)

			{

				if ((Mstream != NULL && fclose(Mstream))

					|| (Qstream != NULL && fclose(Qstream)))

				{

					print_log("[ERROR] �� ������� ������� ����� �������� ������");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

				srand((unsigned int)time(NULL));

				if (n <= 0)

				{

					print_log("[ERROR] �� ������� ������������� ���� � �������� ���������: �� ����� ������ ����������� �������");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

				Mstream = fopen(M_path, "w");

				Qstream = fopen(Q_path, "w");

				if (Mstream == NULL || Qstream == NULL)

				{

					print_err("�� ������� ������������� �����");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

				// ���������� ����� ������ �����

				if (fprintf(Mstream, "%d\n", n) < 0)

				{

					print_err("�� ������� ������������� �����");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					fclose(Mstream);

					fclose(Qstream);

					free(results);

					return 0;

				}

				vsize Msize = n * n;

				weight *M = (weight*)malloc(Msize * sizeof(weight*));

				if (M == NULL)

				{

					print_err("�� ������� �������� ������");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					fclose(Mstream);

					fclose(Qstream);

					free(results);

					return 0;

				}

				for (i = 0; i < n; i++)

				{

					for (j = 0; j < n; j++)

					{

						if (j < i)

							M[i*n + j] = M[j*n + i];

						else if (j == i)

							M[i*n + j] = 0;

						else

							M[i*n + j] = get_random(MIN_WEIGHT, MAX_WEIGHT, (float)DENSITY, INF);

					}

				}

				for (i = 0; i < n; i++)

				{

					for (j = 0; j < n - 1; j++)

					{

						if (fprintf(Mstream, "%d ", M[i*n + j]) < 0)

						{

							print_err("�� ������� ������������� �����");

							MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

							fclose(Mstream);

							fclose(Qstream);

							free(results);

							free(M);

							return 0;

						}

					}

					if (fprintf(Mstream, "%d\n", M[(i + 1) * n - 1]) < 0)

					{

						print_err("�� ������� ������������� �����");

						MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

						fclose(Mstream);

						fclose(Qstream);

						free(results);

						free(M);

						return 0;

					}

				}

				free(M);

				vsize r1, r2;

				do

				{

					r1 = gen_random(1, n);

					r2 = gen_random(1, n);

				} while (n > 1 && r1 == r2);

				if (fprintf(Qstream, "%d -> %d", r1, r2) < 0)

				{

					print_err("�� ������� ������������� �����");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					fclose(Mstream);

					fclose(Qstream);

					free(results);

					return 0;

				}

				if (fclose(Mstream) || fclose(Qstream))

				{

					print_err("�� ������� ������� �����");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

				// �� ��������� �����, ����������� �� ��������� ������

				startTime = MPI_Wtime();

				Mstream = fopen(M_path, "r");

				Qstream = fopen(Q_path, "r");

#ifdef DEBUG

				if (Mstream == NULL || Qstream == NULL)

				{

					print_err("�� ������� ������� �����");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

#endif // DEBUG

			}

#ifdef DEBUG

			// ��������� ����� ������ � �����

			if (fscanf(Mstream, "%d\n", &n) <= 0)

			{

				print_log("[ERROR] �� ������� ������� ����� ������ �����");

				MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

				fclose(Mstream);

				fclose(Qstream);

				free(results);

				return 0;

			}

			// �������� ����� ������

			if (n <= 0)

			{

				print_log("[ERROR] �� ����� ������ ����� ������ �����: |V| = n ������ ���� ������ ����");

				MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

				fclose(Mstream);

				fclose(Qstream);

				free(results);

				return 0;

			}

			// ������ ��������� � �������� ������ �������� ����

			if (fscanf(Qstream, "%d -> %d", &Vstart, &Vend) <= 0)

			{

				print_log("[ERROR] �� ������� ������� ����� ������ �����");

				MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

				fclose(Mstream);

				fclose(Qstream);

				free(results);

				return 0;

			}

			Vstart--; Vend--;

			// �������� ��������� � �������� ������ �������� ����

			if (Vstart < 0 || Vend < 0 || Vstart >= n || Vend >= n)

			{

				print_log("[ERROR] �� ����� ������ ��������� � �������� ������� �������� ����");

				MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

				fclose(Mstream);

				fclose(Qstream);

				free(results);

				return 0;

			}

#else

			fscanf(Mstream, "%d\n", &n);

			fscanf(Qstream, "%d -> %d", &Vstart, &Vend);

			Vstart--; Vend--;

#endif // DEBUG

			fclose(Qstream);

			// ����� ������ ������� W ����� ����������

			Wrows = (vsize *)malloc(procNum * sizeof(vsize));

			Nrows = (vsize *)malloc(procNum * sizeof(vsize));

			p = (vsize)floor(double(n / procNum));

			for (i = 0; i < procNum; i++)

			{

				Wrows[i] = p;

				Nrows[i] = i * p;

			}

			p = n % procNum;

			for (i = 0; p != 0; p--)

			{

				Wrows[i]++;

				for (j = i + 1; j < procNum; j++)

					Nrows[j]++;

				i = (i < procNum) ? i + 1 : 0;

			}

			// ������ ���������� � ������ ������ � ���-����

			if (h == 0)

			{

				print_log("----------------------------------------------\n����� ����������� %d\n����� ������ %d\n������ %f ��\n\n", procNum, n,(double)(n*n * sizeof(weight) + n*n * sizeof(vsize)) / 1024.0 / 1024.0);

#ifdef DEBUG

				print_log("[DEBUG] ������������� ����� ������� M ����� ����������:\n{");

				for (i = 0; i < procNum - 1; i++)

					print_log("%d (%d), ", Wrows[i], Nrows[i]);

				print_log("%d (%d)}\n\n", Wrows[procNum - 1], Nrows[procNum - 1]);

#endif // DEBUG

			}

		}

		vsize Wr;

		vsize Wn;

		if (procRank == MAIN_PROC)

		{

			Wr = Wrows[MAIN_PROC];

			Wn = Nrows[MAIN_PROC];

		}

		// �������� ���� ������������ ����� ������ � ����� � ����� �������������� ����� ������� W

		MPI_Bcast(&n, 1, MPI_VSIZE, MAIN_PROC, MPI_COMM_WORLD);

		MPI_Scatter(Wrows, 1, MPI_VSIZE, &Wr, 1, MPI_VSIZE, MAIN_PROC, MPI_COMM_WORLD);

		MPI_Scatter(Nrows, 1, MPI_VSIZE, &Wn, 1, MPI_VSIZE, MAIN_PROC, MPI_COMM_WORLD);

		vsize Msize = n * n;

		vsize Psize = n * n;

		vsize Ms = Wr * n;

		vsize Ps = Wr * n;

		// ������� ���������

		weight *M = (weight*)malloc(Msize * sizeof(weight));

		// ������� ���������������

		vsize *P = (vsize*)malloc(Psize * sizeof(vsize));

#ifdef DEBUG

		if (M == NULL || P == NULL)

		{

			print_err("�� ������� �������� ������");

			MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

			fclose(Mstream);

			free(results);

			return 0;

		}

#endif // DEBUG

		if (procRank == MAIN_PROC)

		{

			// ������ ������� ���������

			for (i = 0; i < Msize; i++)

				fscanf(Mstream, "%d", &M[i]);

			fclose(Mstream);

			// ������������� ������� ���������������

			for (i = 0; i < n; i++)

			{

				for (j = 0; j < n; j++)

					P[i*n + j] = (M[i*n + j] == INF) ? -1 : i;

			}

		}

		// ������������ ���������� ��������� ������-��������

		for (k = 0; k < n; k++)

		{

			MPI_Bcast(M, Msize, MPI_WEIGHT, MAIN_PROC, MPI_COMM_WORLD);

			MPI_Bcast(P, Psize, MPI_VSIZE, MAIN_PROC, MPI_COMM_WORLD);

			for (i = Wn; i < Wn + Wr; i++)

			{

				for (j = 0; j < n; j++)

				{

					if (M[i*n + j] >(M[i*n + k] + M[k*n + j]))

					{

						P[i*n + j] = P[k*n + j];

						M[i*n + j] = M[i*n + k] + M[k*n + j];

					}

				}

			}

			// �������� ���������� ������ �� ������� �������

			if (procRank != MAIN_PROC)

			{

				MPI_Send(&M[Wn * n], Ms, MPI_WEIGHT, MAIN_PROC, 0, MPI_COMM_WORLD);

				MPI_Send(&P[Wn * n], Ps, MPI_VSIZE, MAIN_PROC, 0, MPI_COMM_WORLD);

			}

			else

			{

				// ���� ���������� ������ �� ������� ��������

				for (p = 0; p < procNum; p++)

				{

					if (p != MAIN_PROC)

					{

						MPI_Recv(&M[Nrows[p] * n], Wrows[p] * n, MPI_WEIGHT, p, MPI_ANY_TAG, MPI_COMM_WORLD, &status);

						MPI_Recv(&P[Nrows[p] * n], Wrows[p] * n, MPI_VSIZE, p, MPI_ANY_TAG, MPI_COMM_WORLD, &status);

					}

				}

			}

		}

#ifdef DEBUG

		if (procRank == MAIN_PROC && !h)

		{

			print_log("[DEBUG] ������� ����� ���������� �����:\n");

			for (i = 0; i < n; i++)

			{

				for (j = 0; j < n - 1; j++)

				{

					if (M[i*n + j] >= INF)

						print_log("- ");

					else

						print_log("%d ", M[i*n + j]);

				}

				if (M[(i + 1)*n - 1] >= INF)

					print_log("-\n");

				else

					print_log("%d\n", M[(i + 1)*n - 1]);

			}

			print_log("\n[DEBUG] ������� ���������������:\n");

			for (i = 0; i < n; i++)

			{

				for (j = 0; j < n - 1; j++)

				{

					if (P[i*n + j] == -1)

						print_log("- ");

					else

						print_log("%d ", P[i*n + j] + 1);

				}

				if (P[(i + 1)*n - 1] == -1)

					print_log("-\n");

				else

					print_log("%d\n", P[(i + 1)*n - 1] + 1);

			}

			print_log("\n");

		}

#endif // DEBUG

		if (procRank == MAIN_PROC)

		{

			// ������� �������� ���� (� �������� �������)

			vsize *path = (vsize*)malloc(n * sizeof(vsize));
			i = 0;

			path[i] = Vend;

			do

			{

				i++;

				if (i < n)

					path[i] = P[Vstart*n + path[i - 1]];

			} while (i < n && path[i] != Vstart);

			// ������ ���������� � ����

			Rstream = fopen(R_path, "w");

			if (M[Vstart*n + Vend] >= INF)

			{

				fprintf(Rstream, "���� �� ������� %d � ������� %d �� ����������\n", Vstart + 1, Vend + 1);

			}

			else

			{

				fprintf(Rstream, "������� ���� (����� %d):\n", M[Vstart*n + Vend]);

				while (i >= 0)

				{

					if (i)

						fprintf(Rstream, "%d => ", path[i] + 1);

					else

						fprintf(Rstream, "%d", path[i] + 1);

					i--;

				}

			}

			fclose(Rstream);

		}

		// ����������� ������

		free(M);

		free(P);

		// ���������� ����� ������ �������� �������� � ���

		if (procRank == MAIN_PROC)

		{

			results[h] = MPI_Wtime() - startTime;

			print_log("����� (%d): %f ���\n", h + 1, results[h]);

		}

}

MPI_Finalize();

if (procRank == MAIN_PROC)

{

	results[0] = results[0] / TEST_COUNT;

	for (i = 1; i < TEST_COUNT; i++)

		results[0] += results[i] / TEST_COUNT;

	print_log("������� : %f ���\n", results[0]);

	free(results);

}

return 0;

}

// ���������� ������-��������� ����� �� ��������� [a;b]

vsize gen_random(vsize a, vsize b)

{

	return (vsize)(a + (b - a) * ((float)rand() / RAND_MAX));

}

// ���������� ������-��������� ����� �� ��������� [a;b], ��������� inf � ������������ (1 - p)

weight get_random(weight a, weight b, float p, weight inf)

{

	return (weight)((((double)rand() / RAND_MAX) >= p) ? inf : (a + (b - a) * ((double)rand() / RAND_MAX)));

}

// ���������� ��������� message � ���

void print_log(const char *message, ...)

{

	va_list ptr;

	va_start(ptr, message);

	// ��������� ���� �����

	FILE *stream = fopen(LOG_PATH, "a");

	if (stream == NULL)

		return;

	// ���������� ��������� � ���

	vfprintf(stream, message, ptr);

	fclose(stream);

	va_end(ptr);

}

// ������� ��������� �� ���� ������ errno

void print_err(const char *message)

{

	char *msg = (char *)"[ERROR] ";

	if (message != NULL)

		msg = strcat(msg, message);

	switch (errno)

	{

	case E2BIG: msg = strcat(msg, "������ ���������� ������� �������"); break;

	case EACCES: msg = strcat(msg, "����� � �������"); break;

	case EADDRINUSE: msg = strcat(msg, "����� ������������"); break;

	case EADDRNOTAVAIL: msg = strcat(msg, "����� ����������"); break;

	case EAFNOSUPPORT: msg = strcat(msg, "��������� ������� �� ��������������"); break;

	case EALREADY: msg = strcat(msg, "���������� ��� ���������������"); break;

	case EBADF: msg = strcat(msg, "������������ ���������� �����"); break;

	case EBADMSG: msg = strcat(msg, "������������ ���������"); break;

	case EBUSY: msg = strcat(msg, "������ �����"); break;

	case ECANCELED: msg = strcat(msg, "�������� ��������"); break;

	case ECHILD: msg = strcat(msg, "��� ��������� ��������"); break;

	case ECONNABORTED: msg = strcat(msg, "���������� ��������"); break;

	case EDEADLK: msg = strcat(msg, "����� ������ ��������"); break;

	case EDESTADDRREQ: msg = strcat(msg, "��������� ����� ����������"); break;

	case EDOM: msg = strcat(msg, "������ ������� �����������"); break;

	case EEXIST: msg = strcat(msg, "���� ����������"); break;

	case EFAULT: msg = strcat(msg, "������������ �����"); break;

	case EFBIG: msg = strcat(msg, "���� ������� �����"); break;

	case EHOSTUNREACH: msg = strcat(msg, "���� ����������"); break;

	case EIDRM: msg = strcat(msg, "������������� ������"); break;

	case EILSEQ: msg = strcat(msg, "��������� ������������������ ������"); break;

	case EINPROGRESS: msg = strcat(msg, "�������� � �������� ����������"); break;

	case EINTR: msg = strcat(msg, "���������� ����� �������"); break;

	case EINVAL: msg = strcat(msg, "������������ ��������"); break;

	case EIO: msg = strcat(msg, "������ �����-������"); break;

	case EISCONN: msg = strcat(msg, "����� (���) ��������"); break;

	case EISDIR: msg = strcat(msg, "��� �������"); break;

	case ELOOP: msg = strcat(msg, "������� ����� ������� ������������� ������"); break;

	case EMFILE: msg = strcat(msg, "������� ����� �������� ������"); break;

	case EMLINK: msg = strcat(msg, "������� ����� ������"); break;

	case EMSGSIZE: msg = strcat(msg, "������������� ����� ������ ���������"); break;

	case ENAMETOOLONG: msg = strcat(msg, "��� ����� ������� �������"); break;

	case ENETDOWN: msg = strcat(msg, "���� �� ��������"); break;

	case ENETRESET: msg = strcat(msg, "���������� �������� �����"); break;

	case ENETUNREACH: msg = strcat(msg, "���� ����������"); break;

	case ENFILE: msg = strcat(msg, "������� ����� �������� ������ � �������"); break;

	case ENOBUFS: msg = strcat(msg, "�������� ������������ ����������"); break;

	case ENODEV: msg = strcat(msg, "��� ������ ����������"); break;

	case ENOENT: msg = strcat(msg, "��� ������ ����� � ��������"); break;

	case ENOEXEC: msg = strcat(msg, "������ ������� ������������ �����"); break;

	case ENOLCK: msg = strcat(msg, "���������� ����������"); break;

	case ENOLINK: msg = strcat(msg, "���������������"); break;

	case ENOMEM: msg = strcat(msg, "������������ ������"); break;

	case ENOMSG: msg = strcat(msg, "��������� ������� ���� �����������"); break;

	case ENOPROTOOPT: msg = strcat(msg, "�������� ����������"); break;

	case ENOSPC: msg = strcat(msg, "������ �� ���������� �� ��������"); break;

	case ENOSYS: msg = strcat(msg, "������� �� �����������"); break;

	case ENOTCONN: msg = strcat(msg, "����� �� ��������"); break;

	case ENOTDIR: msg = strcat(msg, "��� �� �������"); break;

	case ENOTEMPTY: msg = strcat(msg, "������� ��������"); break;

	case ENOTSOCK: msg = strcat(msg, "��� �� �����"); break;

	case ENOTTY: msg = strcat(msg, "������������� �������� ���������� ������-�������"); break;

	case ENXIO: msg = strcat(msg, "��� ������ ���������� ��� ������"); break;

	case EOPNOTSUPP: msg = strcat(msg, "�������� ������ �� ��������������"); break;

	case EOVERFLOW: msg = strcat(msg, "������� ������� �������� ��� ���� ������"); break;

	case EPERM: msg = strcat(msg, "�������� �� ���������"); break;

	case EPIPE: msg = strcat(msg, "����������� �����"); break;

	case EPROTO: msg = strcat(msg, "������ ���������"); break;

	case EPROTONOSUPPORT: msg = strcat(msg, "�������� �� ��������������"); break;

	case EPROTOTYPE: msg = strcat(msg, "��������� ��� ��������� ��� ������"); break;

	case ERANGE: msg = strcat(msg, "��������� ������� �����"); break;

	case EROFS: msg = strcat(msg, "�������� ������� ������ �� ������"); break;

	case ESPIPE: msg = strcat(msg, "������������ ����������������"); break;

	case ESRCH: msg = strcat(msg, "��� ������ ��������"); break;

	case ETIMEDOUT: msg = strcat(msg, "�������� ���������"); break;

	case ETXTBSY: msg = strcat(msg, "��������� ���� �����"); break;

	case EWOULDBLOCK: msg = strcat(msg, "����������� ��������"); break;

	case EXDEV: msg = strcat(msg, "������������� �����"); break;

	default: msg = strcat(msg, "����������� ������");

	}

	print_log(msg);

}