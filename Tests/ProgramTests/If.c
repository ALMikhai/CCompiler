#include <stdio.h>

void print_int(int a) {
    printf("%d\n", a);
}

int main() {
    if (1){
        print_int(1);
        print_int(3);
    }
    else
        print_int(2);        
}