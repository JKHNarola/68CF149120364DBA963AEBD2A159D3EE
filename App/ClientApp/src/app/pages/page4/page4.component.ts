import { Component } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';

@Component({
    selector: 'page-four',
    templateUrl: './page4.component.html',
    animations: [pageSlideUpAnimation]
})
export class Page4Component {
}
