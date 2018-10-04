import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from "../services/app.auth.service";
import { AppConsts } from './app.consts';

@Injectable()
export class RequestInterceptor implements HttpInterceptor {
    constructor(private authService: AuthService) { }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        if (request.headers.has(AppConsts.interceptorSkipAuthHeader)) {
            const headers = request.headers.delete(AppConsts.interceptorSkipAuthHeader);
            return next.handle(request.clone({ headers }));
        }
        else {
            let token = this.authService.getToken();
            if (token != null) {
                request = request.clone({
                    setHeaders: {
                        Authorization: `Bearer ${token}`
                    }
                });
            }
            return next.handle(request);
        }
    }
}
