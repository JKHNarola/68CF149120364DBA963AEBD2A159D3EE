import { Component, OnInit, SimpleChanges, OnChanges } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { BaseApiService } from 'src/app/services/baseapiservice';
import { Query } from '../../misc/query';
import { ProductViewModel } from './productViewModel';

@Component({
    selector: 'page-three',
    templateUrl: './page3.component.html',
    animations: [pageSlideUpAnimation],
    providers: [BaseApiService]
})
export class Page3Component implements OnInit {
    public totalItems: number = 0;

    data: Array<ProductViewModel> = [];
    constructor(private apiService: BaseApiService) {
    }

    ngOnInit(): void {
    }

    getProducts(pageNo, pageSize) {
        let q: Query = new Query(pageNo, pageSize);
        this.apiService.getByParams("api/product/list", { q: q }).subscribe(result => {
            this.totalItems = result.data.total;
            this.data = result.data.data;
        });
    }

    onPageChanged(e) {
        this.getProducts(e.page, e.itemsPerPage);
    }
}
