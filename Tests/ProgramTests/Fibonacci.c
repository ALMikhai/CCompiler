#include <stdio.h>

int fib(int x) {
    if (x == 0)
        return 0;

    if (x == 1)
        return 1;

    return fib(x - 1) + fib(x - 2);
}

void print_int(int a) {
    printf("%d\n", a);
}

int main()
{
    print_int(fib(6));
}
