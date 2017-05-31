// TPR5.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <iostream>
using namespace std;
const int criteriaAmount = 2;
const int solutionsAmount = 10;
int in_array[solutionsAmount][criteriaAmount] = {
	{ 3, 2 },
	{ 5, 6 },
	{ 5, 4 },
	{ 8, 4 },
	{ 6, 2 },
	{ 3, 8 },
	{ 6, 4 },
	{ 2, 5 },
	{ 9, 2 },
	{ 3, 9 }
};
int **outArray, **arrayF1, **arrayF2;
int delta1 = 0, delta2 = 0;
int index1 = 1, index2 = 1;
int result[criteriaAmount];

void FindDominated(int , int , int *);
int** FillOutTheArray(int , int , int x, int *);
unsigned int CountNotDominated(int , int *);
void Sort(int , int , int , int **);


int main() {

	FindDominated(solutionsAmount, criteriaAmount, &in_array[0][0]);
	unsigned int x = CountNotDominated(solutionsAmount, &in_array[0][0]);
	outArray = FillOutTheArray(solutionsAmount, criteriaAmount, x, &in_array[0][0]);

	// вспомогательные массивы для метода уступок
	arrayF1 = new int*[x];
	for (unsigned int i = 0; i < x; i++)
		arrayF1[i] = new int[criteriaAmount];
	arrayF2 = new int*[x];
	for (unsigned int i = 0; i < x; i++)
		arrayF2[i] = new int[criteriaAmount];

	// заполним их
	for (unsigned int i = 0; i < x; i++)
		for (int j = 0; j < criteriaAmount; j++)
			arrayF1[i][j] = arrayF2[i][j] = outArray[i][j];

	// отсортируем  их
	Sort(0, x, criteriaAmount, arrayF1);
	Sort(1, x, criteriaAmount, arrayF2);

	// выведем все наши матрицы
	cout << "Pareto - border:" << endl;
	for (unsigned int i = 0; i < x; i++) {
		for (int j = 0; j < criteriaAmount; j++)
			cout << outArray[i][j] << " ";
		cout << endl;
	}
	cout << "f1max:" << endl;
	for (unsigned int i = 0; i < x; i++) {
		for (int j = 0; j < criteriaAmount; j++)
			cout << arrayF1[i][j] << " ";
		cout << endl;
	}
	cout << "f2max:" << endl;
	for (unsigned int i = 0; i < x; i++) {
		for (int j = 0; j < criteriaAmount; j++)
			cout << arrayF2[i][j] << " ";
		cout << endl;
	}
	//  метод уступок
	int step = 0;
	while (true) {
		step++;
		cout << "Step " << step << ": Res0 == " << result[0] << "; Res1 == " << result[1] << "; delta1 == " << delta1 << "; delta2 == " << delta2 << endl;
		cout << "_________________" << endl;
		if (arrayF1[index1][0] == arrayF2[index2][0] && arrayF1[index1][1] == arrayF2[index2][1]) {
			result[0] = arrayF1[index1][0];
			result[1] = arrayF1[index1][1];
			break;
		}
		if (arrayF1[index1 - 1][0] == arrayF2[index2][0] && arrayF1[index1][1] == arrayF2[index2 - 1][1]) {
			delta1 += (arrayF1[index1 - 1][0] - arrayF1[index1][0]);
			delta2 += (arrayF2[index2 - 1][1] - arrayF2[index2][1]);
			if (delta1 > delta2) {
				result[0] = arrayF1[index1 - 1][0];
				result[1] = arrayF1[index1 - 1][1];
				break;
			}
			if (delta1 < delta2) {
				result[0] = arrayF2[index1 - 1][0];
				result[1] = arrayF2[index1 - 1][1];
				break;
			}
			result[0] = arrayF1[index1 - 1][0];
			result[1] = arrayF1[index1 - 1][1];
			break;
		}
		delta1 += (arrayF1[index1 - 1][0] - arrayF1[index1][0]);
		delta2 += (arrayF2[index2 - 1][1] - arrayF2[index2][1]);
		if (delta1 == delta2) {
			index1++, index2++;
			continue;
		}
		if (delta1 > delta2) {
			index2++;
			continue;
		}
		if (delta1 < delta2) {
			index1++;
			continue;
		}
	}
	cout << "Step " << step++ << ": Res0 == " << result[0] << "; Res1 == " << result[1] << "; delta1 == " << delta1 << "; delta2 == " << delta2 << endl;
	//int solution;
	//for (int i = 0; i < solutionsAmount; i++) {
	//	if (*in_array[i, 0] == result[0] && *in_array[i, 1] == result[1]) {
	//		solution = i;
	//	}
	//}
	cout << "The most perfect solution:" << endl;
	cout <<": f1 = " << result[0] << ", f2 = " << result[1] << endl;
	// освобождение памяти
	for (unsigned int i = 0; i < x; i++) {
		delete[]outArray[i];
		delete[]arrayF1[i];
		delete[]arrayF2[i];
	}
	return 0;
}

void FindDominated(int solutionsAmount, int criteriaAmount, int *matrix) {
	unsigned int i_want_to_eat_smt_delicious;
	unsigned int second_one_wins;
	for (int now = 0; now < solutionsAmount; now++)
		for (int i = 0; i < (solutionsAmount); i++) {
			i_want_to_eat_smt_delicious = second_one_wins = 0;
			for (int j = 0; j < criteriaAmount; j++) {
				if ((*(matrix + now*criteriaAmount + j))>(*(matrix + i*criteriaAmount + j)))
					i_want_to_eat_smt_delicious++;
				else {
					if ((*(matrix + now*criteriaAmount + j)) == (*(matrix + (i)*criteriaAmount + j))) {
						i_want_to_eat_smt_delicious++;
						second_one_wins++;
					}
					else
						second_one_wins++;
				}
			}
			if (i_want_to_eat_smt_delicious == second_one_wins)
				continue;
			else {
				if (i_want_to_eat_smt_delicious > second_one_wins) {
					for (int k = 0; k < criteriaAmount; k++)
						(*(matrix + (i)*criteriaAmount + k)) = -100;
				}
				else
					for (int k = 0; k < criteriaAmount; k++) {
						(*(matrix + now*criteriaAmount + k)) = -100;
						break;
					}
			}
		}
}
int** FillOutTheArray(int solutionsAmount, int criteriaAmount, int x, int *matrix) {
	int **array = new int*[x];
	for (int i = 0; i < x; i++)
		array[i] = new int[criteriaAmount];
	int count = 0;
	for (int i = 0; i < solutionsAmount; i++) {
		if ((*(matrix + i * criteriaAmount)) != -100) {
			for (int j = 0; j < criteriaAmount; j++)
				array[count][j] = *(matrix + i * criteriaAmount + j);
			count++;
		}
	}
	return array;
}
unsigned int CountNotDominated(int solutionsAmount, int *matrix) {
	unsigned int count = 0;
	for (int i = 0; i < solutionsAmount; i++)
		if ((*(matrix + i * criteriaAmount)) != -100)
			count++;
	return count;
}

void Sort(int ind, int solutionsAmount, int criteriaAmount, int **matrix) {
	for (int i = solutionsAmount - 1; i >= 0; i--)
	{
		for (int j = 0; j < i; j++)
		{
			if (matrix[j][ind] < matrix[j + 1][ind])
			{
				int *tmp = new int[criteriaAmount];
				tmp[0] = matrix[j][0];
				tmp[1] = matrix[j][1];
				matrix[j][0] = matrix[j + 1][0];
				matrix[j][1] = matrix[j + 1][1];
				matrix[j + 1][0] = tmp[0];
				matrix[j + 1][1] = tmp[1];
			}
		}
	}
}

