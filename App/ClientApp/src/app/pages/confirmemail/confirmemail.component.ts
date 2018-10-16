import { Component, OnInit } from '@angular/core';
import { AccountService } from '../../services/account.service';
import { Router, ActivatedRoute } from '@angular/router';
import { pageSlideUpAnimation } from '../../misc/page.animation';

@Component({
    selector: 'confirm-email',
    templateUrl: './confirmemail.component.html',
    providers: [AccountService],
    animations: [pageSlideUpAnimation]
})
export class ConfirmEmailComponent implements OnInit {
    loading = false;
    email: string;
    code: string;
    contentTitle = "Email Confirmation";
    contentHeading = "";
    contentMessage = "Loading..";

    constructor(
        private route: ActivatedRoute,
        private accountService: AccountService,
        private router: Router) {
        this.email = this.route.snapshot.queryParams.email;
        this.code = this.route.snapshot.queryParams.code;
    }

    ngOnInit(): void {
        if (this.email && this.code) {
            this.accountService.confirmEmail(this.email, this.code).subscribe(
                result => {
                    if (result.status === 0) {
                        this.contentTitle = "Link Expired!!";
                        this.contentHeading = "The link you are trying to access is expired!!";
                        this.contentMessage = "";
                    } else if (result.status === 1) {
                        this.contentTitle = "Email Confirmed";
                        this.contentHeading = "Final step..";
                        this.contentMessage = "Please check your inbox for final step to set username & password";
                    }
                },
                err => {
                    this.loading = false;
                });
        }
        else {
            this.router.navigate(['/home']);
        }
    }
}
