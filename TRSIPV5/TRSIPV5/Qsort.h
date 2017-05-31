#pragma once
#include <string>

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
			std::swap(mas[l++], mas[r--]);
	}
	if (b < r)
		Qsort(mas, b, r);
	if (e > l)
		Qsort(mas, l, e);
}