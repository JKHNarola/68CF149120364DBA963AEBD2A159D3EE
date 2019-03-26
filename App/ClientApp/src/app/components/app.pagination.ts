import { Component, OnInit, Input, OnChanges, SimpleChanges, EventEmitter, Output } from "@angular/core";

@Component({
    selector: "app-pagination",
    templateUrl: "./app.pagination.html",
})
export class AppPaginationComponent implements OnInit {
    @Input() pageNo: number = 0;
    @Input() pageSize: number = 0;
    @Input() total: number = 0;
    @Output() onPageChanged = new EventEmitter();

    constructor() {
    }

    ngOnInit(): void {
    }

    pageChanged(e) {
        this.pageNo = e.page;
        this.onPageChanged.emit(e);
    }
}
