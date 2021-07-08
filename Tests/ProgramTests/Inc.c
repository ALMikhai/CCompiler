#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    int a = 3;
    ++a;
    print_int(a);
    --a;
    print_int(a); 
}