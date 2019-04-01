import { Component, OnInit, Input, EventEmitter, Output } from "@angular/core";
import { Paginator } from "src/app/misc/query";
import { NgbDropdownConfig } from "@ng-bootstrap/ng-bootstrap";

@Component({
    selector: "app-pagination",
    templateUrl: "./app.pagination.html",
    providers: [NgbDropdownConfig]
})
export class AppPaginationComponent implements OnInit {
    @Input() paginator: Paginator = null;
    @Input() initLoad: boolean = true;
    @Output() getData = new EventEmitter();
    drpItemsPerPage = [5, 10, 25, 50, 100, 500];

    get recordStart() {
        return this.paginator.pageSize * (this.paginator.pageNo - 1) + 1;
    }

    get recordEnd() {
        return this.paginator.pageSize == 0 || this.paginator.totalItems < this.paginator.pageSize * this.paginator.pageNo ? this.paginator.totalItems : (this.paginator.pageSize * this.paginator.pageNo);
    }

    constructor(config: NgbDropdownConfig) {
        config.placement = 'top-left';
        config.autoClose = true;
    }

    ngOnInit(): void {
        if (this.paginator == null)
            this.paginator = new Paginator(1, 50);
        if (!this.paginator.pageNo || this.paginator.pageNo < 0) this.paginator.pageNo = 1;
        if (!this.paginator.pageSize || this.paginator.pageSize < 0) this.paginator.pageSize = 50;
        if (!this.paginator.totalItems || this.paginator.totalItems < 0) this.paginator.totalItems = 0;
        if (!this.paginator.maxSize || this.paginator.maxSize < 0) this.paginator.maxSize = 10;

        if (this.initLoad) this.getData.emit();
    }

    onPageChanged() {
        this.getData.emit();
    }

    onPageSizeChanged(e) {
        this.paginator.pageSize = e;
        if (this.paginator.pageSize == 0) {
            this.paginator.pageNo = 1;
            this.getData.emit();
        }
        else {
            let totalPages = Math.ceil(this.paginator.totalItems / this.paginator.pageSize);
            if (this.paginator.pageNo > totalPages)
                this.paginator.pageNo = totalPages;
            this.getData.emit();
        }
    }
}
