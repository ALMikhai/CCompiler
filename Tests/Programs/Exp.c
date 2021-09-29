#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 3;
    int b = 0;
    a = (1, print_int(a), b = 1, 4);
    print_int(a);
    print_int(b);
}
