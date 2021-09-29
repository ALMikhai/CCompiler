#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 0;
    int b = 1;
    if (a == 0 && b){
        print_int(1);
    }
    if (a == 1 && b){
        print_int(2);
    }
    if (a || b){
        print_int(3);
    }
    if (a == 1 || b){
        print_int(4);
    }
}