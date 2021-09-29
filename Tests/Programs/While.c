#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    while (0) {
        print_int(1);
    }
    
    do {
        print_int(2);
    } while (0);
}