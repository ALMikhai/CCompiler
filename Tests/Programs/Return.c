#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int foo(){
    return 100;
}

int main() {
    print_int(foo());
}