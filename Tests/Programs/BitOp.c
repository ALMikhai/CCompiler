#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 2 & 3;
    print_int(a);
    a = 2 | 3;
    print_int(a);
    a = 2 ^ 3;
    print_int(a);
}
