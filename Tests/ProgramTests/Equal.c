#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a;
    int b;
    b = a = 5;
    print_int(a);
    print_int(b);    
}