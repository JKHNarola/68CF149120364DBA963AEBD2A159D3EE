import { Component, OnChanges, SimpleChanges, Input } from "@angular/core";
import { NgxSpinnerService } from "ngx-spinner";

@Component({
    selector: "app-loader",
    templateUrl: "./app.loader.html",
})
export class AppLoaderComponent implements OnChanges {
    @Input() isLoading: boolean = false;

    constructor(private spinner: NgxSpinnerService) {

    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes["isLoading"]) {
            if (changes["isLoading"].currentValue == true)
                this.spinner.show();
            else
                this.spinner.hide();
        }
    }
}
