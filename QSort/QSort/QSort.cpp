// QSort.cpp: определяет точку входа для консольного приложения.
//

#include "stdafx.h"
#include <iostream>
#include <ctime>
#include "string.h"
using namespace std;

int abs(int a) {
	if (a < 0) return -a;
	return a;
}

int max(int *mas, int left, int right) {
	int maxE = mas[left];
	for (int i = left + 1; i < right; i++) {
		if (maxE < mas[i]) {
			maxE = mas[i];
		}
	}
	return maxE;
}
int min(int *mas, int left, int right) {
	int minE = mas[left];
	for (int i = left + 1; i < right; i++) {
		if (minE > mas[i]) {
			minE = mas[i];
		}
	}
	return minE;
}
void QSort(int *mas, int left, int right) {
	if (abs(left - right) < 2) {
		return;
	}
	int mid = (min(mas, left, right) + max(mas, left, right)) / 2;

	int l = left, r = right;
	while (l <= r) {
		while (mas[l] < mid) {
			l++;
		}
		while (mas[r] > mid) {
			r--;
		}
		if (l <= r) {
			swap(mas[l], mas[r]);
			l++; r--;
		}
	}
	if (left < r) {
		QSort(mas, left, r);
	}
	if (right > l) {
		QSort(mas, l, right);
	}

}
int* RandomizeMas(int n) {
	int *mas = new int[n];
	for (int i = 0; i < n; i++) {
		mas[i] = rand() ;
	}
	return mas;
}
void printMas(int* mas, int n) {
	for (int i = 0; i < n; i++) {
		printf("%i ", mas[i]);
	}
}
void Qsort(int *mas, int b, int e)
{
	int l = b, r = e;
	int piv = mas[(l + r) / 2]; // Опорным элементом для примера возьмём средний
	while (l <= r)
	{
		while (mas[l] < piv)
			l++;
		while (mas[r] > piv)
			r--;
		if (l <= r)
			swap(mas[l++], mas[r--]);
	}
	if (b < r)
		Qsort(mas, b, r);
	if (e > l)
		Qsort(mas, l, e);
}
double test(int n) {
	int* mas = RandomizeMas(n);
	long startTime = clock();
	Qsort(mas, 0, n - 1);
	long endTime = clock();
	
	if (n == 10) {
		printMas(mas, n);
		printf("\n");
	}

	delete mas;
	return (endTime - startTime)*0.001;
}
int main()
{
	printf("Sort time of 1 000 elements (s): %f\n", test(10));
	printf("Sort time of 10 000 elements (s): %f\n", test(10000));
	printf("Sort time of 100 000 elements (s): %f\n", test(100000));
	printf("Sort time of 1 000 000 elements (s): %f\n", test(1000000));
	printf("Sort time of 10 000 000 elements (s): %f\n", test(10000000));
	char с = getchar();
}

