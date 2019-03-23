import { Component } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { BaseApiService } from 'src/app/services/baseapiservice';

@Component({
    selector: 'page-three',
    templateUrl: './page3.component.html',
    animations: [pageSlideUpAnimation],
    providers: [BaseApiService]
})
export class Page3Component {
    constructor(private apiService: BaseApiService) {
        this.apiService.get("api/product/list").subscribe(result => {
            console.log(result);
        });

        //this.apiService.getFile("api/test/download").subscribe(x => {
        //    console.log(x);
        //    // It is necessary to create a new blob object with mime-type explicitly set
        //    // otherwise only Chrome works like it should
        //    var newBlob = new Blob([x], { type: "text/html" });

        //    // IE doesn't allow using a blob object directly as link href
        //    // instead it is necessary to use msSaveOrOpenBlob
        //    if (window.navigator && window.navigator.msSaveOrOpenBlob) {
        //        window.navigator.msSaveOrOpenBlob(newBlob);
        //        return;
        //    }

        //    // For other browsers: 
        //    // Create a link pointing to the ObjectURL containing the blob.
        //    const data = window.URL.createObjectURL(newBlob);

        //    var link = document.createElement('a');
        //    link.href = data;
        //    link.download = "abc.html";
        //    // this is necessary as link.click() does not work on the latest firefox
        //    link.dispatchEvent(new MouseEvent('click', { bubbles: true, cancelable: true, view: window }));

        //    setTimeout(function () {
        //        // For Firefox it is necessary to delay revoking the ObjectURL
        //        window.URL.revokeObjectURL(data);
        //        link.remove();
        //    }, 100);
        //});
    }
}
