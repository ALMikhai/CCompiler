#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 2;
    
    if (a < 1) {
        print_int(1);
    }
    if (a > 1) {
        print_int(2);
    }
    if (a <= 1) {
        print_int(3);
    }
    if (a >= 1) {
        print_int(4);
    }
    
    a = 0;
    if (a < 1) {
        print_int(1);
    }
    if (a > 1) {
        print_int(2);
    }
    if (a <= 1) {
        print_int(3);
    }
    if (a >= 1) {
        print_int(4);
    }
}