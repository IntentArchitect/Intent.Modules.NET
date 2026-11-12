//@IntentMerge()
import { IntentIgnoreBody, IntentMerge, IntentIgnore } from './../../../intent/intent.decorators';
import { CustomersService } from './../../../service-proxies/customers/customers-service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@IntentMerge()
@Component({
  selector: 'app-customer-edit-page',
  standalone: true,
  templateUrl: 'customer-edit-page.component.html'
})
export class CustomerEditPageComponent implements OnInit {
  model: any = null;

  //@IntentMerge()
  constructor(private route: ActivatedRoute, private readonly customersService: CustomersService) {
  }

  @IntentMerge()
  ngOnInit(): void {
    const customerId = this.route.snapshot.paramMap.get('customerId');
    this.customersService.getCustomerById(customerId!).subscribe({
      next: (data) => {
        this.model = data;
      }
    });
  }

  @IntentMerge()
  updateCustomer(): void {
    this.customersService.updateCustomer(this.model.id, this.model).subscribe();
  }
}
