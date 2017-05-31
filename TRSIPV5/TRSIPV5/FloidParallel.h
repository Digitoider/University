
#include <stdio.h>

#include <string.h>

#include <stdlib.h>

#include <errno.h>

#include <time.h>

#include <stdarg.h>

#include <math.h>

#include <mpi.h>

// --- Настройки программы ---

// Включение режима отладки

#define DEBUG

// Число запусков теста

#define TEST_COUNT 5

// Ограничения на генерируемые веса рёбер

#define MIN_WEIGHT 1

#define MAX_WEIGHT 9

// Связность графа

#define DENSITY 0.80

// Число вершин графа

#define V_SIZE 100

// Тип числа вершин в графе

typedef int vsize;

// Тип весов рёбер графа

typedef int weight;

// Путь к файлу логов

#define LOG_PATH "log.txt"

// Определение бесконечного веса ребра

#define INF 100

// --- Настройки MPI ---

// Индекс главного (управляющего) процесса

#define MAIN_PROC 0

// Тип числа вершин в графе

#define MPI_VSIZE MPI_INT

// Тип весов рёбер графа

#define MPI_WEIGHT MPI_INT

weight get_random(weight a, weight b, float density, weight inf);

vsize gen_random(vsize a, vsize b);

void print_log(const char *message, ...);

void print_err(const char *message);

int main(int argc, char *argv[])

{

	// Число процессов и номер текущего процесса

	int procNum = 1;

	int procRank = MAIN_PROC;

	// Время начала работы главного процесса

	double startTime;

	vsize i, j, k, p, h;

	// Число вершин графа G=(V,E), |V|=n

	vsize n = V_SIZE;

	// Номера начальной и конечной вершин искомого пути

	vsize Vstart, Vend;

	MPI_Init(&argc, &argv);

	MPI_Comm_size(MPI_COMM_WORLD, &procNum);

	MPI_Comm_rank(MPI_COMM_WORLD, &procRank);

	MPI_Status status;

	// Пути к файлам матрицы смежности, запросов и результата

	const char *M_path = "M.txt";

	const char *Q_path = "query.txt";

	const char *R_path = "result.txt";

	FILE *Mstream;

	FILE *Qstream;

	FILE *Rstream;

	// Результаты замеров времени работы главного процесса

	double *results;

	vsize *Wrows = NULL;

	vsize *Nrows = NULL;

	if (procRank == MAIN_PROC)

	{

		// Выделение памяти под результаты замеров времени

		results = (double *)malloc(TEST_COUNT * sizeof(double));

		if (results == NULL)

		{

			print_err("Не удалось выделить память");

			MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

			return 0;

		}

	}

	// Запускаем тест TEST_COUNT раз

	for (h = 0; h < TEST_COUNT; h++)

	{

		MPI_Barrier(MPI_COMM_WORLD);

		if (procRank == MAIN_PROC)

		{

			// Фиксируем время начала работы главного процесса

			startTime = MPI_Wtime();

			// Пытаемся открыть файлы с матрицей смежности и запросов

			Mstream = fopen(M_path, "r");

			Qstream = fopen(Q_path, "r");

			// Обработка ошибок при открытии файлов

			if (Mstream == NULL || Qstream == NULL)

			{

				if ((Mstream != NULL && fclose(Mstream))

					|| (Qstream != NULL && fclose(Qstream)))

				{

					print_log("[ERROR] Не удалось закрыть файлы исходных данных");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

				srand((unsigned int)time(NULL));

				if (n <= 0)

				{

					print_log("[ERROR] Не удалось сгенерировать файл с матрицей смежности: Не верно задана размерность матрицы");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

				Mstream = fopen(M_path, "w");

				Qstream = fopen(Q_path, "w");

				if (Mstream == NULL || Qstream == NULL)

				{

					print_err("Не удалось сгенерировать файлы");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

				// Записываем число вершин графа

				if (fprintf(Mstream, "%d\n", n) < 0)

				{

					print_err("Не удалось сгенерировать файлы");

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

					print_err("Не удалось выделить память");

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

							print_err("Не удалось сгенерировать файлы");

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

						print_err("Не удалось сгенерировать файлы");

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

					print_err("Не удалось сгенерировать файлы");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					fclose(Mstream);

					fclose(Qstream);

					free(results);

					return 0;

				}

				if (fclose(Mstream) || fclose(Qstream))

				{

					print_err("Не удалось закрыть файлы");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

				// Не учитываем время, затраченное на генерацию файлов

				startTime = MPI_Wtime();

				Mstream = fopen(M_path, "r");

				Qstream = fopen(Q_path, "r");

#ifdef DEBUG

				if (Mstream == NULL || Qstream == NULL)

				{

					print_err("Не удалось открыть файлы");

					MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

					free(results);

					return 0;

				}

#endif // DEBUG

			}

#ifdef DEBUG

			// Считываем число вершин в графе

			if (fscanf(Mstream, "%d\n", &n) <= 0)

			{

				print_log("[ERROR] Не удалось считать число вершин графа");

				MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

				fclose(Mstream);

				fclose(Qstream);

				free(results);

				return 0;

			}

			// Проверка числа вершин

			if (n <= 0)

			{

				print_log("[ERROR] Не верно задано число вершин графа: |V| = n должно быть больше нуля");

				MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

				fclose(Mstream);

				fclose(Qstream);

				free(results);

				return 0;

			}

			// Чтение начальной и конечной вершин искомого пути

			if (fscanf(Qstream, "%d -> %d", &Vstart, &Vend) <= 0)

			{

				print_log("[ERROR] Не удалось считать число вершин графа");

				MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

				fclose(Mstream);

				fclose(Qstream);

				free(results);

				return 0;

			}

			Vstart--; Vend--;

			// Проверка начальной и конечной вершин искомого пути

			if (Vstart < 0 || Vend < 0 || Vstart >= n || Vend >= n)

			{

				print_log("[ERROR] Не верно заданы начальная и конечная вершины искомого пути");

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

			// Делим строки матрицы W между процессами

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

			// Запись информации о группе тестов в лог-файл

			if (h == 0)

			{

				print_log("----------------------------------------------\nЧисло процессоров %d\nЧисло вершин %d\nПамять %f МБ\n\n", procNum, n,(double)(n*n * sizeof(weight) + n*n * sizeof(vsize)) / 1024.0 / 1024.0);

#ifdef DEBUG

				print_log("[DEBUG] Распределение строк матрицы M между процессами:\n{");

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

		// Рассылка всем исполнителям числа вершин в графе и числа обрабатываемых строк матрицы W

		MPI_Bcast(&n, 1, MPI_VSIZE, MAIN_PROC, MPI_COMM_WORLD);

		MPI_Scatter(Wrows, 1, MPI_VSIZE, &Wr, 1, MPI_VSIZE, MAIN_PROC, MPI_COMM_WORLD);

		MPI_Scatter(Nrows, 1, MPI_VSIZE, &Wn, 1, MPI_VSIZE, MAIN_PROC, MPI_COMM_WORLD);

		vsize Msize = n * n;

		vsize Psize = n * n;

		vsize Ms = Wr * n;

		vsize Ps = Wr * n;

		// Матрица смежности

		weight *M = (weight*)malloc(Msize * sizeof(weight));

		// Матрица предшествования

		vsize *P = (vsize*)malloc(Psize * sizeof(vsize));

#ifdef DEBUG

		if (M == NULL || P == NULL)

		{

			print_err("Не удалось выделить память");

			MPI_Abort(MPI_COMM_WORLD, MPI_ERR_OTHER);

			fclose(Mstream);

			free(results);

			return 0;

		}

#endif // DEBUG

		if (procRank == MAIN_PROC)

		{

			// Чтение матрицы смежности

			for (i = 0; i < Msize; i++)

				fscanf(Mstream, "%d", &M[i]);

			fclose(Mstream);

			// Инициализация матрицы предшествования

			for (i = 0; i < n; i++)

			{

				for (j = 0; j < n; j++)

					P[i*n + j] = (M[i*n + j] == INF) ? -1 : i;

			}

		}

		// Параллельная реализация алгоритма Флойда-Уоршелла

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

			// Отправка фрагментов матриц на главный процесс

			if (procRank != MAIN_PROC)

			{

				MPI_Send(&M[Wn * n], Ms, MPI_WEIGHT, MAIN_PROC, 0, MPI_COMM_WORLD);

				MPI_Send(&P[Wn * n], Ps, MPI_VSIZE, MAIN_PROC, 0, MPI_COMM_WORLD);

			}

			else

			{

				// Приём фрагментов матриц на главном процессе

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

			print_log("[DEBUG] Матрица весов кратчайших путей:\n");

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

			print_log("\n[DEBUG] Матрица предшествования:\n");

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

			// Вершины искомого пути (в обратном порядке)

			vsize *path = (vsize*)malloc(n * sizeof(vsize));
			i = 0;

			path[i] = Vend;

			do

			{

				i++;

				if (i < n)

					path[i] = P[Vstart*n + path[i - 1]];

			} while (i < n && path[i] != Vstart);

			// Запись результата в файл

			Rstream = fopen(R_path, "w");

			if (M[Vstart*n + Vend] >= INF)

			{

				fprintf(Rstream, "Путь из вершины %d в вершину %d не существует\n", Vstart + 1, Vend + 1);

			}

			else

			{

				fprintf(Rstream, "Искомый путь (длина %d):\n", M[Vstart*n + Vend]);

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

		// Освобождаем память

		free(M);

		free(P);

		// Записываем время работы главного процесса в лог

		if (procRank == MAIN_PROC)

		{

			results[h] = MPI_Wtime() - startTime;

			print_log("Время (%d): %f сек\n", h + 1, results[h]);

		}

}

MPI_Finalize();

if (procRank == MAIN_PROC)

{

	results[0] = results[0] / TEST_COUNT;

	for (i = 1; i < TEST_COUNT; i++)

		results[0] += results[i] / TEST_COUNT;

	print_log("Среднее : %f сек\n", results[0]);

	free(results);

}

return 0;

}

// Генерирует псевдо-случайное число из интервала [a;b]

vsize gen_random(vsize a, vsize b)

{

	return (vsize)(a + (b - a) * ((float)rand() / RAND_MAX));

}

// Генерирует псевдо-случайное число из интервала [a;b], возвращая inf с вероятностью (1 - p)

weight get_random(weight a, weight b, float p, weight inf)

{

	return (weight)((((double)rand() / RAND_MAX) >= p) ? inf : (a + (b - a) * ((double)rand() / RAND_MAX)));

}

// Записывает сообщения message в лог

void print_log(const char *message, ...)

{

	va_list ptr;

	va_start(ptr, message);

	// Открываем файл логов

	FILE *stream = fopen(LOG_PATH, "a");

	if (stream == NULL)

		return;

	// Записываем сообщение в лог

	vfprintf(stream, message, ptr);

	fclose(stream);

	va_end(ptr);

}

// Выводит сообщение по коду ошибки errno

void print_err(const char *message)

{

	char *msg = (char *)"[ERROR] ";

	if (message != NULL)

		msg = strcat(msg, message);

	switch (errno)

	{

	case E2BIG: msg = strcat(msg, "Список аргументов слишком длинный"); break;

	case EACCES: msg = strcat(msg, "Отказ в доступе"); break;

	case EADDRINUSE: msg = strcat(msg, "Адрес используется"); break;

	case EADDRNOTAVAIL: msg = strcat(msg, "Адрес недоступен"); break;

	case EAFNOSUPPORT: msg = strcat(msg, "Семейство адресов не поддерживается"); break;

	case EALREADY: msg = strcat(msg, "Соединение уже устанавливается"); break;

	case EBADF: msg = strcat(msg, "Неправильный дескриптор файла"); break;

	case EBADMSG: msg = strcat(msg, "Неправильное сообщение"); break;

	case EBUSY: msg = strcat(msg, "Ресурс занят"); break;

	case ECANCELED: msg = strcat(msg, "Операция отменена"); break;

	case ECHILD: msg = strcat(msg, "Нет дочернего процесса"); break;

	case ECONNABORTED: msg = strcat(msg, "Соединение прервано"); break;

	case EDEADLK: msg = strcat(msg, "Обход тупика ресурсов"); break;

	case EDESTADDRREQ: msg = strcat(msg, "Требуется адрес назначения"); break;

	case EDOM: msg = strcat(msg, "Ошибка области определения"); break;

	case EEXIST: msg = strcat(msg, "Файл существует"); break;

	case EFAULT: msg = strcat(msg, "Неправильный адрес"); break;

	case EFBIG: msg = strcat(msg, "Файл слишком велик"); break;

	case EHOSTUNREACH: msg = strcat(msg, "Хост недоступен"); break;

	case EIDRM: msg = strcat(msg, "Идентификатор удален"); break;

	case EILSEQ: msg = strcat(msg, "Ошибочная последовательность байтов"); break;

	case EINPROGRESS: msg = strcat(msg, "Операция в процессе выполнения"); break;

	case EINTR: msg = strcat(msg, "Прерванный вызов функции"); break;

	case EINVAL: msg = strcat(msg, "Неправильный аргумент"); break;

	case EIO: msg = strcat(msg, "Ошибка ввода-вывода"); break;

	case EISCONN: msg = strcat(msg, "Сокет (уже) соединен"); break;

	case EISDIR: msg = strcat(msg, "Это каталог"); break;

	case ELOOP: msg = strcat(msg, "Слишком много уровней символических ссылок"); break;

	case EMFILE: msg = strcat(msg, "Слишком много открытых файлов"); break;

	case EMLINK: msg = strcat(msg, "Слишком много связей"); break;

	case EMSGSIZE: msg = strcat(msg, "Неопределённая длина буфера сообщения"); break;

	case ENAMETOOLONG: msg = strcat(msg, "Имя файла слишком длинное"); break;

	case ENETDOWN: msg = strcat(msg, "Сеть не работает"); break;

	case ENETRESET: msg = strcat(msg, "Соединение прервано сетью"); break;

	case ENETUNREACH: msg = strcat(msg, "Сеть недоступна"); break;

	case ENFILE: msg = strcat(msg, "Слишком много открытых файлов в системе"); break;

	case ENOBUFS: msg = strcat(msg, "Буферное пространство недоступно"); break;

	case ENODEV: msg = strcat(msg, "Нет такого устройства"); break;

	case ENOENT: msg = strcat(msg, "Нет такого файла в каталоге"); break;

	case ENOEXEC: msg = strcat(msg, "Ошибка формата исполняемого файла"); break;

	case ENOLCK: msg = strcat(msg, "Блокировка недоступна"); break;

	case ENOLINK: msg = strcat(msg, "Зарезервировано"); break;

	case ENOMEM: msg = strcat(msg, "Недостаточно памяти"); break;

	case ENOMSG: msg = strcat(msg, "Сообщение нужного типа отсутствует"); break;

	case ENOPROTOOPT: msg = strcat(msg, "Протокол недоступен"); break;

	case ENOSPC: msg = strcat(msg, "Памяти на устройстве не осталось"); break;

	case ENOSYS: msg = strcat(msg, "Функция не реализована"); break;

	case ENOTCONN: msg = strcat(msg, "Сокет не соединен"); break;

	case ENOTDIR: msg = strcat(msg, "Это не каталог"); break;

	case ENOTEMPTY: msg = strcat(msg, "Каталог непустой"); break;

	case ENOTSOCK: msg = strcat(msg, "Это не сокет"); break;

	case ENOTTY: msg = strcat(msg, "Неопределённая операция управления вводом-выводом"); break;

	case ENXIO: msg = strcat(msg, "Нет такого устройства или адреса"); break;

	case EOPNOTSUPP: msg = strcat(msg, "Операция сокета не поддерживается"); break;

	case EOVERFLOW: msg = strcat(msg, "Слишком большое значение для типа данных"); break;

	case EPERM: msg = strcat(msg, "Операция не разрешена"); break;

	case EPIPE: msg = strcat(msg, "Разрушенный канал"); break;

	case EPROTO: msg = strcat(msg, "Ошибка протокола"); break;

	case EPROTONOSUPPORT: msg = strcat(msg, "Протокол не поддерживается"); break;

	case EPROTOTYPE: msg = strcat(msg, "Ошибочный тип протокола для сокета"); break;

	case ERANGE: msg = strcat(msg, "Результат слишком велик"); break;

	case EROFS: msg = strcat(msg, "Файловая система только на чтение"); break;

	case ESPIPE: msg = strcat(msg, "Неправильное позиционирование"); break;

	case ESRCH: msg = strcat(msg, "Нет такого процесса"); break;

	case ETIMEDOUT: msg = strcat(msg, "Операция задержана"); break;

	case ETXTBSY: msg = strcat(msg, "Текстовый файл занят"); break;

	case EWOULDBLOCK: msg = strcat(msg, "Блокирующая операция"); break;

	case EXDEV: msg = strcat(msg, "Неопределённая связь"); break;

	default: msg = strcat(msg, "Неизвестная ошибка");

	}

	print_log(msg);

}