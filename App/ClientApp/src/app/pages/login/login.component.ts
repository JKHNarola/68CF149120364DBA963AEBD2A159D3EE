import { Component, OnInit } from "@angular/core";
import { AccountService } from "../../services/account.service";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { AuthService } from "../../services/app.auth.service";
import { Router } from "@angular/router";
import { ApiService } from "../../services/apiservice";
import { ToastrService } from "ngx-toastr";
import { pageSlideUpAnimation } from "../../misc/page.animation";

@Component({
    selector: "login",
    templateUrl: "./login.component.html",
    providers: [ApiService, AccountService],
    animations: [pageSlideUpAnimation]
})
export class LoginComponent implements OnInit {
    loginForm: FormGroup;
    loading = false;

    constructor(
        private accountService: AccountService,
        private formBuilder: FormBuilder,
        private authService: AuthService,
        private toastrService: ToastrService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.loginForm = this.formBuilder.group({
            userName: ["", [Validators.required]],
            password: ["", [Validators.required, Validators.minLength(8)]]
        });
    }

    get f() {
        return this.loginForm.controls;
    }

    onLogin() {
        this.loading = true;

        if (this.loginForm.invalid) {
            this.loading = false;
            return;
        }

        let username = this.loginForm.value.userName;
        let password = this.loginForm.value.password;

        this.accountService.login(username, password).subscribe(
            res => {
                if (res.status === 1) {
                    this.authService.setCurrentUser(res.jsonData);
                    this.router.navigate(["/home"]);
                } else if (res.status === 0) {
                    this.toastrService.error("Invalid email or password!!");
                }
                this.loading = false;
            },
            err => {
                this.loading = false;
            }
        );
    }
}
