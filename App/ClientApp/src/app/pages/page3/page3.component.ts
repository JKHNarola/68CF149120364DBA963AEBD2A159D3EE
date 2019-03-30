import { Component, OnInit } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { BaseApiService } from 'src/app/services/baseapiservice';
import { Query, Sort, Paginator } from '../../misc/query';
import { ProductViewModel } from './productViewModel';
import { filter } from 'rxjs/operators';
import { isNumber } from 'util';
import { Operator, Logic } from 'src/app/misc/app.enums';

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
    loading = false;
    filter: any = {
        name: null,
        pricegt: null,
        pricelt: null
    };

    constructor(private apiService: BaseApiService) {
    }

    ngOnInit(): void {
    }

    getProducts() {
        let q: Query = new Query(this.paginator.pageNo, this.paginator.pageSize);
        q.addSort(this.sort.columnName, this.sort.direction);

        if (this.filter.name) 
            q.addCondition("ProductName", this.filter.name.trim(), Operator.StartsWith);
        if (this.filter.pricegt && isNumber(Number(this.filter.pricegt)))
            q.addCondition("UnitPrice", Number(this.filter.pricegt.trim()), Operator.Ge);
        if (this.filter.pricelt && isNumber(Number(this.filter.pricelt)))
            q.addCondition("UnitPrice", Number(this.filter.pricelt.trim()), Operator.Le);

        this.loading = true;
        this.apiService.getByParams("api/product/list", { q: q }).subscribe(result => {
            this.paginator.totalItems = result.data.total;
            this.data = result.data.data;
            this.loading = false;
        });
    }
}
