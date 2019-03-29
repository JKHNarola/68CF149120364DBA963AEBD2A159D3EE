import { Component, OnInit, SimpleChanges, OnChanges } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { BaseApiService } from 'src/app/services/baseapiservice';
import { Query, Sort, Paginator } from '../../misc/query';
import { ProductViewModel } from './productViewModel';

@Component({
    selector: 'page-three',
    templateUrl: './page3.component.html',
    animations: [pageSlideUpAnimation],
    providers: [BaseApiService]
})
export class Page3Component implements OnInit {
    sort: Sort = new Sort(null, null);
    paginator: Paginator = new Paginator(1, 5);
    data: Array<ProductViewModel> = [];

    constructor(private apiService: BaseApiService) {
    }

    ngOnInit(): void {
    }

    getProducts() {
        let q: Query = new Query(this.paginator.pageNo, this.paginator.pageSize);
        q.addSort(this.sort.columnName, this.sort.direction);
        this.apiService.getByParams("api/product/list", { q: q }).subscribe(result => {
            this.paginator.totalItems = result.data.total;
            this.data = result.data.data;
        });
    }
}
