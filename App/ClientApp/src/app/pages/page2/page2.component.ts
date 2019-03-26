import { Component } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { BaseApiService } from '../../services/baseapiservice';
import { Dictionary } from 'src/app/misc/query';

@Component({
    selector: 'page-two',
    templateUrl: './page2.component.html',
    animations: [pageSlideUpAnimation],
    providers: [BaseApiService]
})
export class Page2Component {
    userId = "";
    username = "";
    email = "";
    role = "";

    constructor(private apiService: BaseApiService) {
        this.apiService.get("api/test").subscribe(res => {
            if (res.status === 1) {
                this.userId = res.data.id;
                this.email = res.data.email;
                this.username = res.data.username;
                this.role = res.data.role;
            }
        });

        let pair = new Dictionary<any>();
        pair.add("m", "25")
        this.apiService.getByParams("api/test/error", pair).subscribe(res => {
            console.log(res);
        });

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
