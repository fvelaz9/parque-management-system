import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MantenimientoForm } from './mantenimiento-form';

describe('MantenimientoForm', () => {
  let component: MantenimientoForm;
  let fixture: ComponentFixture<MantenimientoForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MantenimientoForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MantenimientoForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
