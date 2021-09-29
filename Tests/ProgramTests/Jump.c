#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int i = 0;
    for (i = 1; i; i = 0) {
        if (i)
            break;
        print_int(i);
    }
    print_int(i);
}