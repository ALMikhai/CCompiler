#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 1;
    int b = a + 2;
    print_int(b);
    b = a - 12;
    print_int(b);
}
