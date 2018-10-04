import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from "../services/app.auth.service";
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../../environments/environment';

@Injectable()
export class ResponseInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService, private router: Router, private toastr: ToastrService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError((error, caught) => {

            if (!environment.production)
                console.log(error);

            this.handleError(error);
            return throwError(error);
        }) as any);
    }


    private handleError(err: HttpErrorResponse): Observable<any> {
        if (err.status === 401) {
            this.authService.removeCurrentUser();
            this.router.navigate(['/login']);
            return of(err.message);
        }
        else if (err.status === 400) {
            this.toastr.error(this.prepareBadRequestMessage(err));
        }
    }
    private prepareBadRequestMessage(err: HttpErrorResponse): string {
        let msg = "";
        if (err.error.message) {
            msg = "Following request error occured.<br><br>";
            msg = msg + err.error.message;
        }
        else if (err.error.data) {
            msg = "Following request error(s) occured.<br><br>";
            for (let x of err.error.data) {
                for (let y of x) {
                    msg = msg + y.errorMessage + "<br>";
                }
            }
        }
        return msg;
    }
}
