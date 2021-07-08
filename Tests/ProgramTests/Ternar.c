#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 0;
    int b = 1;
    int c = a == 0 ? 1 : a + 2;
    print_int(c);
    c = a != 0 ? 1 : a + 2;
    print_int(c);
}