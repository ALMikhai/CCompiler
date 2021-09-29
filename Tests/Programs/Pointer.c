#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int b = 1;
    int *a = &b;
    int c;
    print_int(*a);
    b = 2;
    print_int(*a);
    *a = 5;
    print_int(b);
    c = *a;
    print_int(c);
}