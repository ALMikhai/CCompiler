#include <stdio.h>

struct a {
    int b;
};

void print_int(int a) {
    printf("%d\n", a);
}

void print_a(struct a a1) {
    printf("%d\n", a1.b);
}

int main() {
    struct a b;
    struct a *a = &b;
    struct a c;
    a->b = 0;
    print_int(a->b);
    
    b.b = 10;
    print_a(*a);

    a->b = 20;
    print_a(b);

    c = *a;
    print_a(c);

    a->b = 60;
    print_a(b);
    print_a(c);
}