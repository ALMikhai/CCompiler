#include <stdio.h>

void print_arr_int(int a[10]) {
    printf("%d ", a[0]);
    printf("%d ", a[1]);
    printf("%d ", a[2]);
    printf("%d ", a[3]);
    printf("%d ", a[4]);
    printf("%d ", a[5]);
    printf("%d ", a[6]);
    printf("%d ", a[7]);
    printf("%d ", a[8]);
    printf("%d ", a[9]);
    printf("\n", 0);
}

void print_arr_int_and_change(int a[10]) {
    a[0] = 1;
    printf("%d ", a[0]);
    printf("%d ", a[1]);
    printf("%d ", a[2]);
    printf("%d ", a[3]);
    printf("%d ", a[4]);
    printf("%d ", a[5]);
    printf("%d ", a[6]);
    printf("%d ", a[7]);
    printf("%d ", a[8]);
    printf("%d ", a[9]);
    printf("\n", 0);
}

int main() {
    int a[10];
    int i = 0;
    for (i = 0; i < 10; i = i + 1)
        a[i] = 0;
    
    a[0] = 4;
    print_arr_int(a);
    print_arr_int_and_change(a);
    print_arr_int(a);
}
