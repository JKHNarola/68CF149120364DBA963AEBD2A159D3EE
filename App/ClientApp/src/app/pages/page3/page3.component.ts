import { Component, OnInit } from '@angular/core';
import { pageSlideUpAnimation } from '../../misc/page.animation';
import { BaseApiService } from 'src/app/services/baseapiservice';
import { Query, Sort, Paginator } from '../../misc/query';
import { ProductViewModel } from './product.viewmodel';
import { isNumber } from 'util';
import { Operator } from 'src/app/misc/app.enums';
import { CategoryViewModel } from './category.viewmodel';

@Component({
    selector: 'page-three',
    templateUrl: './page3.component.html',
    animations: [pageSlideUpAnimation],
    providers: [BaseApiService]
})
export class Page3Component implements OnInit {
    sort: Sort = new Sort();
    paginator: Paginator = new Paginator();
    data: Array<ProductViewModel> = [];
    loading = false;
    filter: any = {
        name: null,
        pricegt: null,
        pricelt: null,
        category: ""
    };
    categories: Array<CategoryViewModel> = [];

    constructor(private apiService: BaseApiService) {
    }

    ngOnInit(): void {
        this.getCategories();
    }

    getProducts() {
        let q: Query = new Query(this.paginator.pageNo, this.paginator.pageSize);
        q.addSort(this.sort);

        if (this.filter.name)
            q.addCondition("ProductName", this.filter.name.trim(), Operator.StartsWith);
        if (this.filter.pricegt && isNumber(Number(this.filter.pricegt)))
            q.addCondition("UnitPrice", Number(this.filter.pricegt.trim()), Operator.Ge);
        if (this.filter.pricelt && isNumber(Number(this.filter.pricelt)))
            q.addCondition("UnitPrice", Number(this.filter.pricelt.trim()), Operator.Le);
        if (this.filter.category) 
            q.addCondition("categoryID", Number(this.filter.category.trim()));

        this.loading = true;
        this.apiService.getByParams("api/product/list", { q: q }).subscribe(result => {
            this.paginator.totalItems = result.data.total;
            this.data = <Array<ProductViewModel>>result.data.data;
            this.loading = false;
        });
    }

    resetFilters() {
        this.filter.name = null;
        this.filter.pricegt = null;
        this.filter.pricelt = null;
        this.filter.category = "";
        this.sort = new Sort();
        this.paginator = new Paginator();
        this.getProducts();
    }

    getCategories() {
        let q: Query = new Query(0, 0);
        q.addExtra("WithImages", false);

        this.apiService.getByParams("api/category/list", { q: q }).subscribe(result => {
            this.categories = <Array<CategoryViewModel>>result.data.data;
            console.log(this.categories);
        });
    }
}
