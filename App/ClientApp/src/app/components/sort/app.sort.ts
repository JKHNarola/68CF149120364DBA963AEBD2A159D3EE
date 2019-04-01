import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Sort } from 'src/app/misc/query';
import { SortOrder } from 'src/app/misc/app.enums';

@Component({
    selector: '[sort]',
    templateUrl: './app.sort.html'
})
export class AppSortComponent implements OnInit {
    @Input() model: Sort;
    @Input() columnName: string;
    @Input() title: string;
    @Output() getData: EventEmitter<any> = new EventEmitter<any>();
    SO = SortOrder;

    constructor() {
    }

    ngOnInit() {
    }

    onSort() {
        this.model.columnName = this.columnName;
        this.model.direction = this.model.direction == SortOrder.Desc ? SortOrder.Asc : SortOrder.Desc;
        this.onGetData();
    };

    onGetData() {
        this.getData.emit(this.model);
    }
}
