import { Injectable } from '@angular/core';

@Injectable()
export class PrintService {
    public print(printEl: HTMLElement) {
        let printContents, popupWin;
        let styles = document.getElementsByTagName("style");
        let styleStr = "";
        Array.from(styles).forEach(x => { styleStr += x.innerHTML; });
        printContents = printEl.innerHTML;
        popupWin = window.open('', '_blank', 'top=0,left=0');
        popupWin.document.open();
        let content =
            `
        <html>
            <head>
                <title>Page Title</title>
                <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css" integrity="sha384-MCw98/SFnGE8fJT3GXwEOngsV7Zt27NXFoaoApmYm81iuXoPkFOJwJ8ERdknLPMO"
                    crossorigin="anonymous">
                <link rel="stylesheet" href="https://use.fontawesome.com/releases/v5.3.1/css/all.css" integrity="sha384-mzrmE5qonljUremFsqc01SB46JvROS7bZs3IO2EmfFsd15uHvIt+Y8vEf7N7fWAU"
                    crossorigin="anonymous">
                <style>
                    ${styleStr}
                    @media print {
                        @page {
                            size: 330mm 427mm;
                            margin: 14mm;
                        }
                        .container {
                            width: 1170px;
                        }
                    }
                </style>
            </head>
            <body onload="window.print();window.close()">${printContents}</body>
        </html>
        `;
        
        // console.log(content);
        popupWin.document.write(content);
        popupWin.document.close();
    }
}