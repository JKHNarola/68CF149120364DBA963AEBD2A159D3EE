import { Component } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { ApiService } from '../../services/apiservice';
import { KeyValuePair } from '../../services/models/keyvalue.model';

@Component({
    selector: 'page-two',
    templateUrl: './page2.component.html',
    animations: [pageSlideUpAnimation],
    providers: [ApiService]
})
export class Page2Component {
    userId = "";
    username = "";
    email = "";
    role = "";

    constructor(private apiService: ApiService) {
        this.apiService.get("api/test").subscribe(res => {
            console.log(res);
            if (res.status === 1) {
                this.userId = res.jsonData.id;
                this.email = res.jsonData.email;
                this.username = res.jsonData.username;
                this.role = res.jsonData.role;
            }
        });

        //let pair = new KeyValuePair();
        //pair.key = "m";
        //pair.value = "25";
        //this.apiService.getByParams("api/test/error", [pair]).subscribe(res => {
        //    console.log(res);
        //});

        //let d = new Data();
        //d.x = "25";
        //this.apiService.post("api/test/error/post", d).subscribe(res => {
        //    console.log(res);
        //});
    }
}

//class Data {
//    x: string;
//    y: string;
//}
