#include <stdio.h>
 
void merge_sort(int i, int j, int a[10], int aux[10]) {
    int mid = (i + j) / 2;
    int pointer_left = i;
    int pointer_right = mid + 1;
    int k;
    
    if (j <= i) {
        return;
    }
    
    merge_sort(i, mid, a, aux);
    merge_sort(mid + 1, j, a, aux);

    for (k = i; k <= j; ++k) {
        if (pointer_left == mid + 1) {
            aux[k] = a[pointer_right];
            ++pointer_right;
        } else if (pointer_right == j + 1) {
            aux[k] = a[pointer_left];
            ++pointer_left;
        } else if (a[pointer_left] < a[pointer_right]) {
            aux[k] = a[pointer_left];
            ++pointer_left;
        } else {
            aux[k] = a[pointer_right];
            ++pointer_right;
        }
    }

    for (k = i; k <= j; ++k) {
        a[k] = aux[k];
    }
}

void init_array(int a[10]) {
    a[0] = -1;
    a[1] = 100;
    a[2] = ~3;
    a[3] = 8;
    a[4] = -12;
    a[5] = -17;
    a[6] = 1;
    a[7] = 10;
    a[8] = 45;
    a[9] = 10;
}

void print_array(int a[10]) {
    int i;
    for (i = 0; i < 10; ++i) {
        printf("%d ", a[i]);
    }
    printf("\n", 0);
}

int main() {
    int a[10], aux[10], n = 10;
    init_array(a);
    print_array(a);
    merge_sort(0, n - 1, a, aux); 
    print_array(a);
}
