#include <stdio.h>

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

int main()
{
    int array[10], n = 10, c, d, swap;
    init_array(array);
    print_array(array);
  
    for (c = 0 ; c < n - 1; ++c)
    {
        for (d = 0 ; d < n - c - 1; ++d)
        {
            if (array[d] > array[d+1])
            {
                swap       = array[d];
                array[d]   = array[d+1];
                array[d+1] = swap;
            }
        }
    }

    print_array(array);
}
