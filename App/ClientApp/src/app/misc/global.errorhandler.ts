import { Injectable, ErrorHandler } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable()
export class GlobalErrorHandler extends ErrorHandler {
    constructor(private toastr: ToastrService) {
        super();
    }

    handleError(error: any) {
        if (error instanceof HttpErrorResponse) {
            //Do not need to handle here
        }
        else if (error instanceof TypeError) {
            if (!environment.production) {
                console.error("type error", error.stack);
                this.toastr.error("TypeError occurred. Please check console.");
            }
            else
                this.toastr.error("Something went wrong. Please contact your admin or try again. Code:TypeError");
        }
        else if (error instanceof ErrorEvent) {
            //A client-side or network error occurred. Handle it accordingly.
            if (!environment.production) {
                console.error("error event", error.message);
                this.toastr.error("ErrorEvent occurred. Please check console.");
            }
            else
                this.toastr.error("Something went wrong. Please contact your admin or try again. Code:ErrorEvent");
        }
        else {
            if (!environment.production) {
                console.error("error", error.stack);
                this.toastr.error("Error occurred. Please check console.");
            }
            else
                this.toastr.error("Something went wrong. Please contact your admin or try again.");
        }

        //throw error;
    }
}

