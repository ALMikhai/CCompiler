#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 3;
    int b = a * 20;
    print_int(b);
    b = b / 2;
    print_int(b);
    b = b % 20;
    print_int(b);
}
