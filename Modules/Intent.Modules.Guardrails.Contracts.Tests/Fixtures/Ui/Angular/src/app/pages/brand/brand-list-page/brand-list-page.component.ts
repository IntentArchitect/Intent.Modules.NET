//@IntentMerge()
import { IntentIgnoreBody, IntentMerge, IntentIgnore } from './../../../intent/intent.decorators';
import { BrandDto } from './../../../service-proxies/models/backend/services/brands/brand-dto';
import { BrandsService } from './../../../service-proxies/brands/brands-service';
import { Component, OnInit, inject } from '@angular/core';
import { finalize } from 'rxjs';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { BrandAddDialogComponent } from './../brand-add-dialog/brand-add-dialog.component';

@IntentMerge()
@Component({
  selector: 'app-brand-list-page',
  standalone: true,
  templateUrl: 'brand-list-page.component.html',
  imports: [CommonModule, MatTableModule, MatDialogModule]
})
export class BrandListPageComponent implements OnInit {
  isLoading = false;
  brandsModels: BrandDto[] | null = null;
  private dialog = inject(MatDialog);

  //@IntentMerge()
  constructor(private readonly brandsService: BrandsService) {
  }

  @IntentMerge()
  ngOnInit(): void {
    this.loadBrands();
  }

  @IntentMerge()
  addBrand(): void {
    const dialogRef = this.dialog.open(BrandAddDialogComponent, {
      width: '600px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.loadBrands();
      }
    });
  }

  @IntentMerge()
  loadBrands(): void {
    this.isLoading = true;
    this.brandsService.getBrands()
      .pipe(finalize(() => { this.isLoading = false; }))
      .subscribe({
        next: (data) => {
          this.brandsModels = data;
        }
      });
  }
}
