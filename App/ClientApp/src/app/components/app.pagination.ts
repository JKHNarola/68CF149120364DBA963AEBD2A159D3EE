import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";

@Component({
    selector: "app-pagination",
    templateUrl: "./app.pagination.html",
})
export class AppPaginationComponent implements OnInit {
    @Input() pageNo: number = -1;
    @Input() pageSize: number = -1;
    @Input() total: number = -1;
    @Output() onPageChanged = new EventEmitter();
    totalPages: number = 0;

    constructor() {
    }

    ngOnInit(): void {
    }

    pageChanged(e) {
        if (this.pageSize > -1 && this.pageNo > -1)
            this.onPageChanged.emit(e);
        else
            this.pageNo = 1;
    }

    numPages(e) {
        if (this.pageSize > -1 && this.pageNo > -1 && this.pageNo <= e) {
            this.totalPages = e;
            this.onPageChanged.emit({ page: this.pageNo, itemsPerPage: this.pageSize });
        }
    }
}
