import { Component } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { ApiService } from '../../services/apiservice';

@Component({
    selector: 'page-two',
    templateUrl: './page2.component.html',
    animations: [pageSlideUpAnimation],
    providers: [ApiService]
})
export class Page2Component {
    constructor(private apiService: ApiService) {
        this.apiService.get("api/test").subscribe(res => {
            console.log(res);
        });
    }
}
