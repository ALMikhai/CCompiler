#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 1;
    int b = a << 3;
    print_int(b);
    b = b >> 2;
    print_int(b);
}