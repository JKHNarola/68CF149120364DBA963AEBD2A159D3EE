import { Component } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';

@Component({
    selector: 'page-three',
    templateUrl: './page3.component.html',
    animations: [pageSlideUpAnimation]
})
export class Page3Component {
}
