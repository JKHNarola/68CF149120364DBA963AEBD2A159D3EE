import { trigger, animate, transition, style } from '@angular/animations';

export const pageSlideUpAnimation =
    trigger('pageSlideUpAnimation', [
        transition(':enter', [
            style({
                opacity: 0,
                transform: 'translateY(10px)'
            }),
            animate(
                '.3s cubic-bezier(0.680, -0.100, 0.265, 1.50)',
                style({
                    opacity: 1,
                    transform: 'translateY(0)'
                })
            )
        ]),
    ]);