//@IntentMerge()
import { IntentIgnoreBody, IntentMerge, IntentIgnore } from './../../../intent/intent.decorators';
import { BrandsService } from './../../../service-proxies/brands/brands-service';
import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { finalize } from 'rxjs';

interface CreateBrandModel {
  name: string;
}

@IntentMerge()
@Component({
  selector: 'app-brand-add-dialog',
  standalone: true,
  templateUrl: 'brand-add-dialog.component.html',
  imports: [CommonModule, FormsModule, MatDialogModule]
})
export class BrandAddDialogComponent {
  isLoading = false;
  model: CreateBrandModel = {
    name: ''
  };

  //@IntentMerge()
  constructor(private dialogRef: MatDialogRef<BrandAddDialogComponent>, private readonly brandsService: BrandsService) {
  }

  @IntentIgnore()
  createBrand(): void {
    this.isLoading = true;
    this.brandsService.createBrand(this.model.name)
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe({
        next: () => {
          this.dialogRef.close(true);
        }
      });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
