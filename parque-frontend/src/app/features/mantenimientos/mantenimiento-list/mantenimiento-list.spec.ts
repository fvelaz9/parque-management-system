import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MantenimientoList } from './mantenimiento-list';

describe('MantenimientoList', () => {
  let component: MantenimientoList;
  let fixture: ComponentFixture<MantenimientoList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MantenimientoList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MantenimientoList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
