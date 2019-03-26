import { Component, OnInit } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { BaseApiService } from 'src/app/services/baseapiservice';
import { Query, Dictionary, Paginator } from '../../misc/query';
import { Operator, SortOrder } from 'src/app/misc/app.enums';
import { ProductViewModel } from './productViewModel';

@Component({
    selector: 'page-three',
    templateUrl: './page3.component.html',
    animations: [pageSlideUpAnimation],
    providers: [BaseApiService]
})
export class Page3Component implements OnInit {
    paginator = new Paginator(10, 1, 0);
    data: Array<ProductViewModel> = [];
    constructor(private apiService: BaseApiService) {
    }

    ngOnInit(): void {
        this.getProducts();
    }

    getProducts() {
        let q: Query = new Query(this.paginator.pageNo, this.paginator.pageSize);
        this.apiService.getByParams("api/product/list", { q: q }).subscribe(result => {
            this.paginator.totalItems = result.data.total;
            this.data = result.data.data;

            console.log(this.data);
        });
    }

    onPageChanged(e) {
        this.paginator.pageNo = e.page;
        this.paginator.pageSize = e.itemsPerPage;
        this.getProducts();
    }
}
