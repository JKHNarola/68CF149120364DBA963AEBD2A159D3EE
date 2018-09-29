import { Component } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';

@Component({
    selector: 'page-two',
    templateUrl: './page2.component.html',
    animations: [pageSlideUpAnimation]
})
export class Page2Component {
}
